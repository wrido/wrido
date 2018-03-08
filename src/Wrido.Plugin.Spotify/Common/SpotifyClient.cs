using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wrido.Logging;
using Wrido.Plugin.Spotify.Authorization;
using Wrido.Plugin.Spotify.Common.Model;
using Wrido.Plugin.Spotify.Common.Model.Full;
using Wrido.Plugin.Spotify.Common.Playback;
using Wrido.Plugin.Spotify.Common.RecentlyPlayed;
using Wrido.Plugin.Spotify.Common.Search;
using Wrido.Plugin.Spotify.Common.Utils;

namespace Wrido.Plugin.Spotify.Common
{
  public interface ISpotifyClient
  {
    bool CanAuthenticate { get; }
    Task<SearchResult> SearchAsync(SearchQuery query, CancellationToken ct = default);
    Task<CursorBasedPagingResult<PlayHistory>> GetRecentlyPlayedAsync(RecentlyPlayedQuery query, CancellationToken ct = default);
    Task<OperationResult> PlayAsync(PlaybackRequest request, CancellationToken ct = default);
    Task<CurrentPlayback> GetCurrentPlaybackAsync(CancellationToken ct = default);
    Task<Album> GetAlbumAsync(string albumId, CancellationToken ct);
    Task<OperationResult> PauseAsync(CancellationToken ct = default);
  }

  public class SpotifyClient : ISpotifyClient
  {
    private readonly IAccessTokenProvider _accessTokenProvider;
    private readonly IQueryParameterBuilder _queryParameterBuilder;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializer _serializer;
    private readonly ILogger _logger;
    private static readonly Uri ApiBaseUrl = new Uri("https://api.spotify.com/v1");

    public bool CanAuthenticate => _accessTokenProvider.IsReady;

    public SpotifyClient(IAccessTokenProvider accessTokenProvider, IQueryParameterBuilder queryParameterBuilder, HttpClient httpClient, JsonSerializer serializer, ILogger logger)
    {
      _accessTokenProvider = accessTokenProvider;
      _queryParameterBuilder = queryParameterBuilder;
      _httpClient = httpClient;
      _serializer = serializer;
      _logger = logger;
    }

    public Task<Album> GetAlbumAsync(string albumId, CancellationToken ct)
    {
      var queryUrl = $"{ApiBaseUrl}/albums{albumId}";
      return GetAsync<Album>(queryUrl, ct);
    }

    public Task<SearchResult> SearchAsync(SearchQuery query, CancellationToken ct = default)
    {
      var queryUrl = $"{ApiBaseUrl}/search{_queryParameterBuilder.Build(query)}";
      return GetAsync<SearchResult>(queryUrl, ct);
    }

    public Task<CursorBasedPagingResult<PlayHistory>> GetRecentlyPlayedAsync(RecentlyPlayedQuery query, CancellationToken ct = default)
    {
      var queryUrl = $"{ApiBaseUrl}/me/player/recently-played{ _queryParameterBuilder.Build(query)}";
      return GetAsync<CursorBasedPagingResult<PlayHistory>>(queryUrl, ct);
    }

    public async Task<OperationResult> PlayAsync(PlaybackRequest request, CancellationToken ct = default)
    {
      var requestUrl = $"{ApiBaseUrl}/me/player/play{ _queryParameterBuilder.Build(request)}";

      var response = await _httpClient.PutAsync(requestUrl, new JsonContent(request, _serializer), ct);
      switch (response.StatusCode)
      {
        case HttpStatusCode.NoContent: return OperationResult.Success;
        case HttpStatusCode.Accepted: return OperationResult.DeviceUnavailable;
        case HttpStatusCode.NotFound: return OperationResult.DeviceNotFound;
        case HttpStatusCode.Forbidden: return OperationResult.NonPremiumUser;
        case HttpStatusCode.BadRequest:
          var error = await DeserializeBodyAsync<UnsuccessfulOperation>(response);
          throw new SpotifyException("Unable to authenticate", error.Error);
        default: return OperationResult.Unknown;
      }
    }

    public async Task<OperationResult> PauseAsync(CancellationToken ct = default)
    {
      var requestUrl = $"{ApiBaseUrl}/me/player/pause";
      var response = await _httpClient.PutAsync(requestUrl, null, ct);
      switch (response.StatusCode)
      {
        case HttpStatusCode.NoContent: return OperationResult.Success;
        case HttpStatusCode.Accepted: return OperationResult.DeviceUnavailable;
        case HttpStatusCode.NotFound: return OperationResult.DeviceNotFound;
        case HttpStatusCode.Forbidden: return OperationResult.NonPremiumUser;
        case HttpStatusCode.BadRequest:
          var error = await DeserializeBodyAsync<UnsuccessfulOperation>(response);
          throw new SpotifyException("Unable to authenticate", error.Error);
        default: return OperationResult.Unknown;
      }
    }

    public Task<CurrentPlayback> GetCurrentPlaybackAsync(CancellationToken ct = default)
    {
      var requestUrl = $"{ApiBaseUrl}/me/player";
      return GetAsync<CurrentPlayback>(requestUrl, ct);
    }

    private async Task<TSpotifyResource> GetAsync<TSpotifyResource>(string queryUrl, CancellationToken ct)
    {
      if (string.IsNullOrWhiteSpace(_httpClient.DefaultRequestHeaders?.Authorization?.Parameter))
      {
        await UpdateAuthorizationHeaderAsync();
      }

      var response = await _httpClient.GetAsync(queryUrl, ct);

      if (response.StatusCode == HttpStatusCode.Unauthorized)
      {
        await UpdateAuthorizationHeaderAsync();
        response = await _httpClient.GetAsync(queryUrl, ct);
        if (!response.IsSuccessStatusCode)
        {
          var error = await DeserializeBodyAsync<UnsuccessfulOperation>(response);
          throw new SpotifyException("Unable to authenticate", error.Error);
        }
      }

      if (!response.IsSuccessStatusCode)
      {
        var error = await DeserializeBodyAsync<UnsuccessfulOperation>(response);
        throw new SpotifyException("Request to Spotify's API was not successful", error.Error);
      }

      var result = await DeserializeBodyAsync<TSpotifyResource>(response);
      return result;
    }

    private async Task UpdateAuthorizationHeaderAsync()
    {
      const string authScheme = "Bearer";
      var token = await _accessTokenProvider.GetAsync();
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authScheme, token);
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
