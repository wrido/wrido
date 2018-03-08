using System.Collections.Generic;
using Wrido.Plugin.Spotify.Common.Model;

namespace Wrido.Plugin.Spotify.Common.Search
{
  public class SearchResult
  {
    public PaginationResult<Model.Full.Artist> Artists { get; set; }
    public PaginationResult<Model.Simplified.Album> Albums { get; set; }
    public PaginationResult<Model.Full.Track> Tracks { get; set; }
  }
}
