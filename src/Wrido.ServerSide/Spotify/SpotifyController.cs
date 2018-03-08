using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Wrido.ServerSide.Spotify
{
  public class SpotifyController : Controller
  {
    private readonly SpotifyOptions _options;
    private readonly IHubContext<SpotifyHub> _hubContext;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializer _serializer;

    public SpotifyController(SpotifyOptions options, IHubContext<SpotifyHub> hubContext)
    {
      _options = options;
      _hubContext = hubContext;
      _httpClient = new HttpClient();
      _serializer = new JsonSerializer
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()}
      };
      _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.ClientSecret}")));
    }

    [HttpGet("spotify/callback")]
    public async Task<IActionResult> HandleAuthorizationCallbackAsync([FromQuery] string code, [FromQuery] string error, [FromQuery] string state)
    {
      if (string.IsNullOrEmpty(state))
      {
        return Ok("State not found");
      }

      var caller = _hubContext.Clients.Client(state);
      if (caller == null)
      {
        return Ok("Caller not found");
      }

      if (!string.IsNullOrEmpty(error))
      {
        await caller.NotifyAuthorizationError(error);
        return Ok($"Authorization failed: {error}");
      }

      var body = new Dictionary<string, string>
      {
        {"grant_type", "authorization_code"},
        {"code", code},
        {"redirect_uri", _options.AuthorizeRedirectUrl.ToString()}
      };
      var response = await _httpClient.PostAsync(_options.AccessTokenUrl, new FormUrlEncodedContent(body));
      var responseBody = await response.Content.ReadAsStringAsync();

      if (!response.IsSuccessStatusCode)
      {
        await caller.NotifyAuthorizationError(response.ReasonPhrase);
        return Ok($"Authorization failed: {responseBody}");
      }

      SpotifyAccess access;
      using (var stringReader = new StringReader(responseBody))
      using (var jsonReader = new JsonTextReader(stringReader))
      {
        try
        {
          access = _serializer.Deserialize<SpotifyAccess>(jsonReader);
        }
        catch (Exception e)
        {
          await caller.NotifyAuthorizationError(e.Message);
          return Ok($"Serialization failed: {e.Message}");
        }
      }

      await caller.SendAccessObjectAsync(access);
      return Json(access);
    }

    [HttpGet("spotify/refresh")]
    public async Task<IActionResult> RefreshAccessTokenAsync([FromQuery] string token)
    {
      var body = new Dictionary<string, string>
      {
        {"grant_type", "refresh_token"},
        {"refresh_token", token},
        {"redirect_uri", _options.AuthorizeRedirectUrl.ToString()}
      };
      var response = await _httpClient.PostAsync(_options.AccessTokenUrl, new FormUrlEncodedContent(body));
      var responseBody = await response.Content.ReadAsStringAsync();

      if (!response.IsSuccessStatusCode)
      {
        return Ok($"Authorization failed: {responseBody}");
      }

      return Ok(responseBody);
    }
  }
}
