using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Wrido.Logging;

namespace Wrido.Plugin.Wikipedia.Common
{
  public interface IWikipediaClient
  {
    Task<SearchResult> SearchAsync(string searchTerm, CancellationToken ct = default);
    Task<PageResult> GetAsync(string pageTitle, CancellationToken ct = default);
    Task<IEnumerable<PageResult>> GetAsync(IEnumerable<string> pageTitles, CancellationToken ct = default);
    Uri BaseUri { get;  }
  }

  public class WikipediaClient : IWikipediaClient
  {
    private readonly HttpClient _httpClient;
    private readonly JsonSerializer _serializer;
    private const string _searchPath = "/w/api.php?action=opensearch&profile=fuzzy&limit=5&search=";
    private const string _pagePath = "/w/api.php?action=query&prop=pageimages|extracts|pageviews|pageterms&pvipdays=1&format=json&piprop=original&exlimit=1&exintro&titles=";
    private readonly ILogger _logger = LogManager.GetLogger<WikipediaClient>();
    public Uri BaseUri => _httpClient.BaseAddress;

    public WikipediaClient(HttpClient httpClient, JsonSerializer serializer)
    {
      _httpClient = httpClient;
      _serializer = serializer;
    }

    public async Task<SearchResult> SearchAsync(string searchTerm, CancellationToken ct = default)
    {
      var requestUrl = $"{_searchPath}{Encode(searchTerm)}";
      var searchResult = await GetResultAsync<SearchResult>(requestUrl, ct);
      _logger.Information("The search phrase {term} resulted in {suggestionCount} suggestions.", searchResult.Term, searchResult.Suggestions.Count);
      return searchResult;
    }

    public async Task<PageResult> GetAsync(string pageTitle, CancellationToken ct = default)
    {
      var resuls = await GetAsync(new[] {pageTitle}, ct);
      return resuls.FirstOrDefault();
    }

    public async Task<IEnumerable<PageResult>> GetAsync(IEnumerable<string> pageTitles, CancellationToken ct = default)
    {
      var requestUrl = $"{_pagePath}{Encode(string.Join("|", pageTitles))}";
      var batchResult = await GetResultAsync<BatchResult>(requestUrl, ct);
      var pageResult = batchResult.Query.Pages.Values;
      return pageResult;
    }

    private async Task<TResult> GetResultAsync<TResult>(string requestUrl, CancellationToken ct) where TResult : new()
    {
      HttpResponseMessage response;
      using (_logger.Timed("Request to {requestUrl}", requestUrl))
      {
        response = await _httpClient.GetAsync(requestUrl, ct);
        ct.ThrowIfCancellationRequested();
      }

      if (!response.IsSuccessStatusCode)
      {
        _logger.Information("Request to {requestUrl} resultet in {statusCode}, reason: {reason}",
          requestUrl, response.StatusCode, response.ReasonPhrase);
        return new TResult();
      }

      var jsonContent = await response.Content.ReadAsStringAsync();
      using (var textReader = new StringReader(jsonContent))
      using (var jsonReader = new JsonTextReader(textReader))
      {
        try
        {
          return _serializer.Deserialize<TResult>(jsonReader);
        }
        catch (Exception e)
        {
          _logger.Error(e, "An unhandled error occured when deserializing response.");
          return default;
        }
        finally
        {
          response.Dispose();
        }
      }
    }

    private static string Encode(string term)
    {
      return WebUtility.UrlEncode(
        term.Replace(" ", "_")
          .Replace("#", "♯"));
    }

    private class BatchResult
    {
      public string BatchComplete { get; set; }
      public QueryResult Query { get; set; }

      public class QueryResult
      {
        public Dictionary<long, PageResult> Pages { get; set; }
      }
    }
  }
}
