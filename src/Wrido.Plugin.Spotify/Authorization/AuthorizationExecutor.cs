using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wrido.Configuration;
using Wrido.Execution;
using Wrido.Logging;
using Wrido.Plugin.Spotify.Common.Authorization;
using Wrido.Queries;

namespace Wrido.Plugin.Spotify.Authorization
{
  public class AuthorizationExecutor : IResultExecuter
  {
    private readonly IAppConfiguration _config;
    private readonly WridoAccessTokenProvider _tokenProvider;
    private readonly ILogger _logger;
    private const string authorizeCallback = "authorizeUrlAvailable";
    private const string authorizeFailed = "authorizeFailed";
    private const string authorizeSucceeded = "authorizeSucceeded";
    private const string startAuthorization = "StartAuthorizationAsync";

    public AuthorizationExecutor(IConfigurationProvider config, WridoAccessTokenProvider tokenProvider, ILogger logger)
    {
      _config = config.GetAppConfiguration();
      _tokenProvider = tokenProvider;
      _logger = logger;
    }

    public bool CanExecute(QueryResult result)
    {
      return result is AuthorizationRequiredResult;
    }

    public async Task ExecuteAsync(QueryResult result)
    {
      if (result is AuthorizationRequiredResult)
      {
        var authOperation = TimedOperationExtensions.Timed(_logger, "Spotify authentication");
        var connection = GetConnectionToSignalR();

        try
        {
          var authorizeCompletion = new TaskCompletionSource<SpotifyAccess>();
          await connection.StartAsync();
          HubConnectionExtensions.On<string>(connection, authorizeCallback, OpenInBrowser.Url);
          HubConnectionExtensions.On<string>(connection, authorizeFailed, s => authorizeCompletion.TrySetException(new Exception($"Spotify authorization failed: {s}")));
          HubConnectionExtensions.On<SpotifyAccess>(connection, authorizeSucceeded, access => authorizeCompletion.TrySetResult(access));
          await HubConnectionExtensions.SendAsync(connection, startAuthorization);
          await authorizeCompletion.Task;
          authOperation.Complete();
          _tokenProvider.Initialize(authorizeCompletion.Task.Result);
        }
        catch (Exception e)
        {
          authOperation.Cancel(e);
        }
        finally
        {
          await connection.StopAsync();
        }
      }
    }

    private HubConnection GetConnectionToSignalR()
    {
      return new HubConnectionBuilder()
        .WithUrl($"{_config.ServerUrl}spotify")
        .WithJsonProtocol(new JsonHubProtocolOptions
        {
          PayloadSerializerSettings = new JsonSerializerSettings
          {
            ContractResolver = new CamelCasePropertyNamesContractResolver
            {
              NamingStrategy = new SnakeCaseNamingStrategy()
            },
            TypeNameHandling = TypeNameHandling.None
          }
        })
        .WithTransport(TransportType.WebSockets)
        .Build();
    }
  }
}
