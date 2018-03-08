using System;
using System.Collections.Generic;

namespace Wrido.Plugin.Spotify.Common.Model
{
  public class TrackLink
  {
    /// <summary>
    /// Known external URLs for this track.
    /// </summary>
    public IDictionary<string, Uri> ExternalUrls { get; set; }

    /// <summary>
    /// A link to the Web API endpoint providing full details of the track.
    /// </summary>
    public Uri Href { get; set; }

    /// <summary>
    /// The Spotify ID for the track. 
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 	The object type: "track".
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The Spotify URI for the track.
    /// </summary>
    public string Uri { get; set; }
  }
}