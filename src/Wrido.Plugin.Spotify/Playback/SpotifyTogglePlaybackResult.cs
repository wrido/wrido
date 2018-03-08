using System;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido.Plugin.Spotify.Playback
{
  public class SpotifyTogglePlaybackResult : QueryResult
  {
    public bool IsPlaying { get; set; }

    private static readonly Image SpotifyLogo = new Image
    {
      Uri = new Uri("/resources/wrido/plugin/spotify/resources/spotify.png", UriKind.Relative),
      Alt = "Spotify"
    };

    public SpotifyTogglePlaybackResult()
    {
      Icon = SpotifyLogo;
    }
  }
}
