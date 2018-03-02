using System;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido.Plugin.Spotify
{
  public class SpotifyAuthRequiredResult : QueryResult
  {
    private static readonly Image SpotifyLogo = new Image
    {
      Uri = new Uri("/resources/wrido/plugin/spotify/resources/spotify.png", UriKind.Relative),
      Alt = "Spotify"
    };

    public SpotifyAuthRequiredResult()
    {
      Icon = SpotifyLogo;
    }
  }
}
