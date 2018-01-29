using System;
using System.Net;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido.Plugin.Google
{
  public class GoogleResult : WebResult
  {
    private static readonly ImageResource googleLogo = new ImageResource
    {
      Uri = new Uri("/resources/wrido/plugin/google/resources/google.png", UriKind.Relative),
      Alt = "Google",
      Key = ResourceKeys.Icon
    };

    public static GoogleResult Fallback => new GoogleResult
    {
      Title = "Open Google in browser",
      Description = "https://www.google.com",
      Uri = new Uri("https://www.google.com"),
      Image = googleLogo
    };

    public static GoogleResult SearchResult(string query)
    {
      var googleUrl = new Uri($"https://www.google.com/search?q={WebUtility.UrlEncode(query)}");
      return new GoogleResult
      {
        Title = $"Search Google for '{query}'",
        Description = googleUrl.AbsoluteUri,
        Uri = googleUrl,
        Image = googleLogo
      };
    }
  }
}
