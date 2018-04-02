using System;
using System.Collections.Generic;
using Wrido.Queries;

namespace Wrido.Plugin.Spotify.Playback
{
  public class PlayableAlbumResult : QueryResult, IPlayableResource
  {
    public IEnumerable<string> ResourceUris { get; set; }
    public string ContextUri { get; set; }
    public string AlbumName { get; set; }
    public string ReleaseDate { get; set; }
    public string ArtistName { get; set; }
    public string ActiveTrackName { get; set; }
    public string CoverArt { get; set; }

    public PlayableAlbumResult()
    {
      PreviewUri = new Uri("/resources/wrido/plugin/spotify/resources/album.htm", UriKind.Relative);
      Icon = SpotifyPlayableResult.SpotifyLogo;
    }
  }
}
