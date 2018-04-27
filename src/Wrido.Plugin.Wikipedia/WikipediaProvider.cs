using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Logging;
using Wrido.Plugin.Wikipedia.Common;
using Wrido.Queries;

namespace Wrido.Plugin.Wikipedia
{
  public class WikipediaProvider : QueryProvider
  {
    private readonly IEnumerable<IWikipediaClient> _clients;
    private readonly WikipediaConfiguration _config;
    private readonly ILogger _logger;

    public WikipediaProvider(IEnumerable<IWikipediaClient> clients, WikipediaConfiguration config, ILogger logger)
    {
      _clients = clients;
      _config = config;
      _logger = logger;
    }

    public override bool CanHandle(IQuery query)
    {
      return string.Equals(query.Command, _config.Keyword, StringComparison.InvariantCultureIgnoreCase);
    }

    protected override async Task QueryAsync(IQuery query, CancellationToken ct)
    {
      if (string.IsNullOrWhiteSpace(query.Argument))
      {
        var fallbacks = WikipediaResult.CreateFallback(_config.BaseUrls);
        Available(fallbacks);
        return;
      }

      var queryTasks = _clients
        .Select(client => QueryWikipediaAsync(client, query.Argument, ct))
        .ToList();
      await Task.WhenAll(queryTasks);
      ct.ThrowIfCancellationRequested();

      var results = queryTasks.SelectMany(q => q.Result.Suggestions);

      if (query is DefaultQuery)
      {
        results = results.Take(3);
      }

      if (results.Any() || query is DefaultQuery)
      {
        Available(WikipediaResult.Create(results));
      }
      else
      {
        var searchResults = WikipediaResult.CreateSearch(Encode(query.Argument), _config.BaseUrls);
        Available(searchResults);
      }
    }

    private async Task<SearchResult> QueryWikipediaAsync(IWikipediaClient client, string searchTerm, CancellationToken ct)
    {
      var searchResult = await client.SearchAsync(searchTerm, ct);
      _logger.Information("The search phrase {term} resulted in {suggestionCount} suggestions.", searchResult.Term, searchResult.Suggestions.Count);
      return searchResult;
    }

    private static string Encode(string term)
    {
      return WebUtility.UrlEncode(
        term.Replace(" ", "_")
          .Replace("#", "♯"));
    }
  }
}
