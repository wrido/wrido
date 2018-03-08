using System;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido.Plugin.Spotify.Playback
{
  public class SpotifyPlayableResult : QueryResult
  {
    public string ResourceUri { get; set; }
    public string ContextUri { get; set; }

    private static readonly Image SpotifyLogo = new Image
    {
      Uri = new Uri("/resources/wrido/plugin/spotify/resources/spotify.png", UriKind.Relative),
      Alt = "Spotify"
    };

    public SpotifyPlayableResult()
    {
      Icon = SpotifyLogo;
    }
  }
}
