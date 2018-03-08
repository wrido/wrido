using System;

namespace Wrido.Plugin.Spotify.Common.Model
{
  public class Followers
  {
    /// <summary>
    /// 	A link to the Web API endpoint providing full details of the followers; null if not available.
    /// Please note that this will always be set to null, as the Web API does not support it at the moment.
    /// </summary>
    public Uri Href { get; set; }

    /// <summary>
    /// 	The total number of followers.
    /// </summary>
    public int Total { get; set; }
  }
}