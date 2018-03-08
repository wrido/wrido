using System;
using System.Collections.Generic;

namespace Wrido.Plugin.Spotify.Common.Model
{
  public class PlayContext
  {
    /// <summary>
    /// The object type, e.g. "artist", "playlist", "album".
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// A link to the Web API endpoint providing full details of the track.
    /// </summary>
    public Uri Href { get; set; }

    /// <summary>
    /// External URLs for this context.
    /// </summary>
    public IDictionary<string, Uri> ExternalUrls { get; set; }

    /// <summary>
    /// The Spotify URI for the context.
    /// </summary>
    public string Uri { get; set; }
  }
}
