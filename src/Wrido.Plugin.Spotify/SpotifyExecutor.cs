using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wrido.Configuration;
using Wrido.Execution;
using Wrido.Logging;
using Wrido.Plugin.Spotify.Authorization;
using Wrido.Queries;

namespace Wrido.Plugin.Spotify
{
  public class SpotifyExecutor : IResultExecuter
  {
    private readonly IAppConfiguration _config;
    private readonly ILogger _logger;
    private readonly HttpClient _httpClient;
    private const string authorizeCallback = "authorizeUrlAvailable";
    private const string authorizeFailed = "authorizeFailed";
    private const string authorizeSucceeded = "authorizeSucceeded";
    private const string startAuthorization = "StartAuthorizationAsync";

    public SpotifyExecutor(IConfigurationProvider config, ILogger logger)
    {
      _config = config.GetAppConfiguration();
      _logger = logger;
      _httpClient = new HttpClient();
    }

    public bool CanExecute(QueryResult result)
    {
      return result is SpotifyAuthRequiredResult;
    }

    public async Task ExecuteAsync(QueryResult result)
    {
      if (result is SpotifyAuthRequiredResult)
      {
        var authOperation = _logger.Timed("Spotify authentication");
        var connection = GetConnectionToSignalR();

        try
        {
          var authorizeCompletion = new TaskCompletionSource<SpotifyAccess>();
          await connection.StartAsync();
          connection.On<string>(authorizeCallback, OpenInBrowser.Url);
          connection.On<string>(authorizeFailed, s => authorizeCompletion.TrySetException(new Exception($"Spotify authorization failed: {s}")));
          connection.On<SpotifyAccess>(authorizeSucceeded, access => authorizeCompletion.TrySetResult(access));
          await connection.SendAsync(startAuthorization);
          await authorizeCompletion.Task;
          authOperation.Complete();

          var response = await _httpClient.GetAsync($"{_config.ServerUrl}spotify/refresh?token={authorizeCompletion.Task.Result.RefreshToken}");
          var responseBody = await response.Content.ReadAsStringAsync();
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
