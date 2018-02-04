using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wrido.Logging;
using Wrido.Queries;
using Wrido.Queries.Events;

namespace Wrido.Plugin.Google
{
  public class GoogleProvider : Queries.IQueryProvider
  {
    protected const string Command = ":g";
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    public GoogleProvider(HttpClient httpClient, ILogger logger)
    {
      _httpClient = httpClient;
      _logger = logger;
    }

    public bool CanHandle(Query query)
    {
      return string.Equals(query.Command, Command, StringComparison.OrdinalIgnoreCase);
    }

    public async Task QueryAsync(Query query, IObserver<QueryEvent> observer, CancellationToken ct)
    {
      if (string.IsNullOrEmpty(query.Argument))
      {
        _logger.Verbose("No search term entered, returning fallback result.");
        observer.OnNext(new ResultAvailable(GoogleResult.Fallback));
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

      foreach (var suggestion in suggestions)
      {
        observer.OnNext(new ResultAvailable(GoogleResult.SearchResult(suggestion)));
      }
    }
  }
}
