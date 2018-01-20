using System;
using System.Net;
using Wrido.Core;

namespace Wrido.Plugin.Google
{
  public class GoogleResult : QueryResult
  {
    public Uri Uri { get; set; }

    public static GoogleResult Fallback => new GoogleResult
    {
      Title = "Open Google in browser",
      Description = "https://www.google.com",
      Uri = new Uri("https://www.google.com")
    };

    public static GoogleResult SearchResult(string query)
    {
      var googleUrl = new Uri($"http://www.google.com/search?q={WebUtility.UrlEncode(query)}");
      return new GoogleResult
      {
        Title = $"Search Google for '{query}'",
        Description = googleUrl.AbsoluteUri,
        Uri = googleUrl
      };
    }
  }
}
