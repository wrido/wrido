using System;

namespace Wrido.ServerSide.Spotify
{
  public class SpotifyOptions
  {
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public Uri AuthorizeUrl { get; set; }
    public Uri AuthorizeRedirectUrl { get; set; }
    public Uri AccessTokenUrl { get; set; }
  }
}
