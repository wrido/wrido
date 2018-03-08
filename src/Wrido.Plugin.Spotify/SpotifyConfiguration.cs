using System;
using Wrido.Configuration;

namespace Wrido.Plugin.Spotify
{
  public class SpotifyConfiguration : IPluginConfiguration
  {
    public string Keyword { get; set; }
    public Uri RefreshAccessUri { get; set; }
    public string RefreshToken { get; set; }

    public static SpotifyConfiguration Default => new SpotifyConfiguration
    {
      Keyword = ":s"
    };
  }
}
