namespace Wrido.Plugin.Spotify.Common.Authorization
{
  public class SpotifyAccess
  {
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public string Scope { get; set; }
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
  }
}
