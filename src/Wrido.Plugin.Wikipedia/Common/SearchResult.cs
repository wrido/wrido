using System;
using System.Collections.Generic;

namespace Wrido.Plugin.Wikipedia.Common
{
  public class SearchResult
  {
    public string Term { get; set; }
    public List<WikipediaSuggestion> Suggestions { get; set; }

    public SearchResult()
    {
      Suggestions = new List<WikipediaSuggestion>();
    }

    public class WikipediaSuggestion
    {
      public string Title { get; set; }
      public string Description { get; set; }
      public Uri Uri { get; set; }
    }
  }
}
