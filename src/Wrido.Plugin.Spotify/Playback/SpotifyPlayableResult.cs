using System;
using System.Collections.Generic;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido.Plugin.Spotify.Playback
{
  public class SpotifyPlayableResult : QueryResult, IPlayableResource
  {
    public IEnumerable<string> ResourceUris { get; set; }
    public int? ResourceTrackNumber { get; set; }
    public string ContextUri { get; set; }
    public string CoverArt { get; set; }
    public string ArtistImage { get; set; }
    public string Artist { get; set; }
    public string TrackName { get; set; }

    public static readonly Image SpotifyLogo = new Image
    {
      Uri = new Uri("/resources/wrido/plugin/spotify/resources/spotify.png", UriKind.Relative),
      Alt = "Spotify"
    };

    public SpotifyPlayableResult()
    {
      Icon = SpotifyLogo;
      PreviewUri = new Uri("/resources/wrido/plugin/spotify/resources/playable.htm", UriKind.Relative);
    }
  }
}
