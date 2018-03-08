using System;
using System.IO;
using System.Net.Http;
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

    public bool IsReady => !string.IsNullOrEmpty(_refreshToken);

    public WridoAccessTokenProvider(HttpClient client, JsonSerializer serializer, SpotifyConfiguration config, ILogger logger)
    {
      _client = client;
      _serializer = serializer;
      _config = config;
      _logger = logger;
      _refreshToken = config.RefreshToken;
    }

    public void Initialize(SpotifyAccess access)
    {
      _refreshToken = access.RefreshToken;
      _nextAccessToken = access.AccessToken;
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

      var response = await _client.GetAsync($"{_config.RefreshAccessUri}?token={_refreshToken}");

      if (!response.IsSuccessStatusCode)
      {
        throw new Exception();
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
          LoggerExtensions.Information(_logger, e, "Unable to deserialize content to {bodyType}", typeof(TBody));
          return default;
        }
      }
    }
  }
}
