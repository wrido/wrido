using System;
using System.Collections.Generic;
using System.Linq;
using Wrido.Plugin.Wikipedia.Common;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido.Plugin.Wikipedia
{
  public class WikipediaResult : WebResult
  {
    private static readonly Image _wikiLogo = new Image
    {
      Uri = new Uri("/resources/wrido/plugin/wikipedia/resources/wikipedia.png", UriKind.Relative),
      Alt = "Wikipedia"
    };

    private const string _wikipediaCategory = "Wikipedia";

    public static IEnumerable<WikipediaResult> Create(IEnumerable<SearchResult.WikipediaSuggestion> suggestions)
    {
      foreach (var suggestion in suggestions)
      {
        yield return new WikipediaResult
        {
          Title = suggestion.Title,
          Description = suggestion.Description,
          Uri = suggestion.Uri,
          PreviewUri = new Uri($"https://en.m.wikipedia.org{suggestion.Uri.PathAndQuery}"),
          Icon = _wikiLogo,
          Category = _wikipediaCategory
        };
      }
    }

    public static IEnumerable<WikipediaResult> CreateFallback(IEnumerable<string> baseUrls)
    {
      return baseUrls.Select(url =>
        new WikipediaResult
        {
          Title = "Open Wikipedia in browser",
          Description = url,
          Uri = new Uri(url),
          Icon = _wikiLogo,
          Category = _wikipediaCategory
        });
    }

    public static IEnumerable<WikipediaResult> CreateSearch(string term, IEnumerable<string> baseUrls)
    {
      return baseUrls.Select(url =>
        new WikipediaResult
        {
          Title = $"Search Wikipedia for '{term}'.",
          Description = $"{url}/wiki/{term}",
          Uri = new Uri(url),
          Icon = _wikiLogo,
          Category = _wikipediaCategory
        });
    }
  }
}
