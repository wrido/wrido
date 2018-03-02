namespace Wrido.Plugin.Spotify.Common
{
  public interface ISpotifyClient
  {
    bool IsAuthenticated { get; }
  }

  public class SpotifyClient : ISpotifyClient
  {
    public bool IsAuthenticated { get; }
  }
}
