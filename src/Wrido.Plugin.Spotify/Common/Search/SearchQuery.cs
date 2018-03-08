using System;

namespace Wrido.Plugin.Spotify.Common.Search
{
  public class SearchQuery
  {
    public string Query { get; set; }

    public SearchType Type { get; set; }

    /// <summary>
    /// An ISO 3166-1 alpha-2 country code or the string from_token.
    /// </summary>
    public string Market { get; set; }

    /// <summary>
    /// The maximum number of results to return. Default: 20. Minimum: 1. Maximum: 50. 
    /// </summary>
    public ushort Limit { get; set; }

    /// <summary>
    /// The index of the first result to return.
    /// Default: 0 (i.e., the first result).
    /// Maximum offset: 100.000. Use with limit to get the next page of search results. 
    /// </summary>
    public int Offset { get; set; }
  }

  [Flags]
  public enum SearchType
  {
    Album = 1,
    Artist = 2,
    Playlist = 4,
    Track = 8,
    All = 15
  }
}
