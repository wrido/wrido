using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wrido.Logging;
using Wrido.Queries;

namespace Wrido.Plugin.Google
{
  public class GoogleProvider : QueryProvider
  {
    protected const string Command = ":g";
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public GoogleProvider(HttpClient httpClient, ILogger logger)
    {
      _httpClient = httpClient;
      _logger = logger;
    }

    public override bool CanHandle(IQuery query)
    {
      return string.Equals(query.Command, Command, StringComparison.OrdinalIgnoreCase);
    }

    protected override async Task QueryAsync(IQuery query, CancellationToken ct)
    {
      if (string.IsNullOrEmpty(query.Argument))
      {
        _logger.Verbose("No search term entered, returning fallback result.");
        Available(GoogleResult.Fallback);
        return;
      }

      var response = await _httpClient.GetAsync($"?output=chrome&q={WebUtility.UrlEncode(query.Argument)}", ct);
      ct.ThrowIfCancellationRequested();

      if (!response.IsSuccessStatusCode)
      {
        return;
      }

      var data = await response.Content.ReadAsStringAsync();
      var suggestions = JArray.Parse(data)[1]
        .Where(x => x.Type == JTokenType.String).Select(x => x.Value<string>())
        .ToList();

      if (!suggestions.Any())
      {
        return;
      }

      var googleSuggestions = suggestions.Select(GoogleResult.SearchResult);
      Available(googleSuggestions);
    }
  }
}
