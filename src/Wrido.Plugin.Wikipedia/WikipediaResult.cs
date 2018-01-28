using System;
using System.Collections.Generic;
using System.Linq;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido.Plugin.Wikipedia
{
  public class WikipediaResult : WebResult
  {
    private static readonly ImageResource Logo = new ImageResource
    {
      Uri = new Uri("/resources/wrido/plugin/wikipedia/resources/wikipedia.png", UriKind.Relative),
      Alt = "Google",
      Key = ResourceKeys.Icon
    };

    public static IEnumerable<WikipediaResult> Create(WikipediaResponse response)
    {
      foreach (var suggestion in response.Suggestions)
      {
        yield return new WikipediaResult
        {
          Title = suggestion.Title,
          Description = suggestion.Description,
          Uri = suggestion.Uri,
          Resources = new[] {Logo}
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
          Resources = new[] {Logo}
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
          Resources = new[] { Logo }
        });
    }
  }
}
