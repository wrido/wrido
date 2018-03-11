using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wrido.Logging;
using Wrido.Plugin.Spotify.Common.Authorization;

namespace Wrido.Plugin.Spotify.Authorization
{
  public class WridoAccessTokenProvider : IAccessTokenProvider
  {
    private readonly HttpClient _client;
    private readonly JsonSerializer _serializer;
    private readonly SpotifyConfiguration _config;
    private readonly ILogger _logger;
    private string _refreshToken;
    private string _nextAccessToken;
    private readonly object _nextTokenLock = new object();
    private readonly TaskCompletionSource<bool> _authorizationTsc;

    public bool IsReady => !string.IsNullOrEmpty(_refreshToken);

    public WridoAccessTokenProvider(HttpClient client, JsonSerializer serializer, SpotifyConfiguration config, ILogger logger)
    {
      _client = client;
      _serializer = serializer;
      _config = config;
      _logger = logger;
      _authorizationTsc = new TaskCompletionSource<bool>();
      _refreshToken = config.RefreshToken;
      if (!string.IsNullOrEmpty(_refreshToken))
      {
        _authorizationTsc.SetResult(true);
      }
    }

    public void Initialize(SpotifyAccess access)
    {
      _refreshToken = access.RefreshToken;
      _nextAccessToken = access.AccessToken;
      _authorizationTsc.TrySetResult(true);
    }

    public Task WaitUntilReadyAsync(CancellationToken ct = default)
    {
      var waitTcs = new TaskCompletionSource<bool>();
      _authorizationTsc.Task.ContinueWith(t => waitTcs.TrySetResult(true), ct);
      ct.Register(() => waitTcs.TrySetCanceled());
      return waitTcs.Task;
    }

    public async Task<string> GetAsync()
    {
      if (!string.IsNullOrEmpty(_nextAccessToken))
      {
        lock (_nextTokenLock)
        {
          if (!string.IsNullOrWhiteSpace(_nextAccessToken))
          {
            var next = _nextAccessToken;
            _nextAccessToken = null;
            return next;
          }
        }
      }

      if (string.IsNullOrEmpty(_refreshToken))
      {
        throw new Exception("No refresh token found.");
      }

      var response = await _client.GetAsync($"{_config.RefreshAccessUri}?token={_refreshToken}");

      if (!response.IsSuccessStatusCode)
      {
        throw new Exception($"Unable to aquire access token. Reason {response.ReasonPhrase}");
      }

      var access = await DeserializeBodyAsync<RefreshedAccess>(response);
      return access.AccessToken;
    }

    private async Task<TBody> DeserializeBodyAsync<TBody>(HttpResponseMessage response)
    {
      var responseBody = await response.Content.ReadAsStringAsync();
      using (var textReader = new StringReader(responseBody))
      using (var jsonReader = new JsonTextReader(textReader))
      {
        try
        {
          return _serializer.Deserialize<TBody>(jsonReader);
        }
        catch (Exception e)
        {
          _logger.Information(e, "Unable to deserialize content to {bodyType}", typeof(TBody));
          return default;
        }
      }
    }
  }
}
