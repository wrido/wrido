using System.Collections.Generic;
using Wrido.Plugin.Spotify.Common.Model.Full;

namespace Wrido.Plugin.Spotify.Common.RecentlyPlayed
{
  public class ArtistTopTracks
  {
    public string ArtistId { get; set; }
    public string CountryCode { get; set; }
    public IList<Track> Tracks { get; set; }
  }
}
