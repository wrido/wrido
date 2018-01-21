using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Wrido.Core.Queries;
using Wrido.Logging;
using Wrido.Queries;

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

    public async Task<IEnumerable<QueryResult>> QueryAsync(Query query, CancellationToken ct)
    {
      if (string.IsNullOrEmpty(query.Argument))
      {
        _logger.Verbose("No search term entered, returning fallback result.");
        return new QueryResult[] { GoogleResult.Fallback };
      }

      var result = await _httpClient.GetAsync($"?output=chrome&q={WebUtility.UrlEncode(query.Argument)}", ct);
      ct.ThrowIfCancellationRequested();

      if (!result.IsSuccessStatusCode)
      {
        return Enumerable.Empty<QueryResult>();
      }

      var data = await result.Content.ReadAsStringAsync();
      var queryResults = JArray.Parse(data)[1]
        .Where(x => x.Type == JTokenType.String).Select(x => x.Value<string>())
        .ToList();

      return queryResults.Any()
        ? queryResults.Select(GoogleResult.SearchResult)
        : new[] { GoogleResult.SearchResult(query.Argument)};
    }
  }
}
