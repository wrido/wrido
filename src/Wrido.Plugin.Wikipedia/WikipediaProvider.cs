using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Logging;
using Wrido.Plugin.Wikipedia.Common;
using Wrido.Queries;
using Wrido.Queries.Events;
using Wrido.Resources;
using IQueryProvider = Wrido.Queries.IQueryProvider;

namespace Wrido.Plugin.Wikipedia
{
  public class WikipediaProvider : IQueryProvider
  {
    private readonly IEnumerable<IWikipediaClient> _clients;
    private readonly ILogger _logger;
    private readonly WikipediaConfiguration _config;

    public WikipediaProvider(IEnumerable<IWikipediaClient> clients, WikipediaConfiguration config, ILogger logger)
    {
      _clients = clients;
      _logger = logger;
      _config = config;
    }

    public bool CanHandle(Query query)
    {
      return string.Equals(query.Command, _config.Keyword, StringComparison.InvariantCultureIgnoreCase);
    }

    public async Task QueryAsync(Query query, IObserver<QueryEvent> observer, CancellationToken ct)
    {
      if (string.IsNullOrWhiteSpace(query.Argument))
      {
        foreach (var fallback in WikipediaResult.CreateFallback(_config.BaseUrls))
        {
          observer.ResultAvailable(fallback);
        }
        return;
      }

      var queryTasks = _clients
        .Select(client => QueryWikipediaAsync(client, query.Argument, observer, ct))
        .ToList();
      await Task.WhenAll(queryTasks);

      var results = queryTasks.SelectMany(q => q.Result.Suggestions).ToList();
      if (!results.Any())
      {
        foreach (var searchResult in WikipediaResult.CreateSearch(Encode(query.Argument), _config.BaseUrls))
        {
          observer.ResultAvailable(searchResult);
        }
      }
    }

    private async Task<SearchResult> QueryWikipediaAsync(IWikipediaClient client, string searchTerm, IObserver<QueryEvent> observer, CancellationToken ct)
    {
      var searchResult = await client.SearchAsync(searchTerm, ct);
      _logger.Information("The search phrase {term} resulted in {suggestionCount} suggestions.", searchResult.Term, searchResult.Suggestions.Count);
      var results = WikipediaResult.Create(searchResult).ToList();

      foreach (var result in results)
      {
        _logger.Verbose("Wikipedia result {title} available", result.Title);
        observer.ResultAvailable(result);
      }

      var pageTitles = results.Select(r => r.Title);
      var pages = await client.GetAsync(pageTitles, ct);
      var pagesByTitle = pages.ToDictionary(page => page.Title, page => page);
      foreach (var result in results)
      {
        if (!pagesByTitle.ContainsKey(result.Title))
        {
          _logger.Warning("Unable to find {pageTitle} in title dictionary", result.Title);
          continue;
        }
        _logger.Verbose("Page {pageTitle} found. Enriching result", result.Title);
        var pageResult = pagesByTitle[result.Title];
        result.Extract = pageResult.Extract;
        result.PageImage = new Image { Uri = pageResult.Original?.Source, Alt = result.Title};
        result.Views = pageResult.PageViews.Values.FirstOrDefault() ?? 0;
        observer.ResultUpdated(result);
      }
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
