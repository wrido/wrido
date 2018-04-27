using System;
using System.Net;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido.Plugin.Google
{
  public class GoogleResult : WebResult
  {
    private const string _googleCategory = "Internet search";

    private GoogleResult() { }

    public GoogleResult(string term)
    {
      if (Uri.TryCreate(term, UriKind.Absolute, out var uri))
      {
        Title = term;
        Uri = uri;
        Category = _googleCategory;
        Icon = Icons.LinkIcon;
      }
      else
      {
        Title = $"Search Google for '{term}'";
        Uri = new Uri($"https://www.google.com/search?q={WebUtility.UrlEncode(term)}");
        Icon = Icons.GoogleLogo;
        Category = _googleCategory;
      }
    }

    public static GoogleResult Fallback => new GoogleResult
    {
      Title = "Open Google in browser",
      Description = "https://www.google.com",
      Uri = new Uri("https://www.google.com"),
      Icon = Icons.GoogleLogo,
      Category = _googleCategory
    };
  }
}
