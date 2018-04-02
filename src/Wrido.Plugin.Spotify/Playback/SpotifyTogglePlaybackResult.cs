using System;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido.Plugin.Spotify.Playback
{
  public class SpotifyTogglePlaybackResult : QueryResult
  {
    public bool IsPlaying { get; set; }
    public string TrackDuration { get; set; }
    public string PlaybackProgress { get; set; }
    public string AlbumName { get; set; }
    public string ReleaseDate { get; set; }
    public string ArtistName { get; set; }
    public string ActiveTrackName { get; set; }
    public string CoverArt { get; set; }

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
