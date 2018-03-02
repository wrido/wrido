using System;
using Wrido.Configuration;

namespace Wrido.Plugin.Spotify
{
  public class SpotifyConfiguration : IPluginConfiguration
  {
    public string Keyword { get; set; }

    public Uri GivePermissionUri { get; set; }
    public Uri AccessTokenUri { get; set; }

    public static SpotifyConfiguration Default => new SpotifyConfiguration
    {
      Keyword = ":s"
    };
  }
}
