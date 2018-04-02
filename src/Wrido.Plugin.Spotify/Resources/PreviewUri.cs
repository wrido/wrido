using System;

namespace Wrido.Plugin.Spotify.Resources
{
  public class PreviewUri
  {
    private const string baseUrl = "/resources/wrido/plugin/spotify/resources";

    public static readonly Uri NowPlaying = new Uri($"{baseUrl}/nowplaying.htm", UriKind.Relative);
    public static readonly Uri Album = new Uri($"{baseUrl}/album.htm", UriKind.Relative);
    public static readonly Uri Artist = new Uri($"{baseUrl}/artist.htm", UriKind.Relative);
    public static readonly Uri Track = new Uri($"{baseUrl}/track.htm", UriKind.Relative);
  }
}
