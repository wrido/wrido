using System.Collections.Generic;

namespace Wrido.Plugin.Spotify.Common.Model.Simplified
{
  public class Artist
  {
    /// <summary>
    /// Known external URLs for this artist.
    /// </summary>
    public IDictionary<string, string> ExternalUrls { get; set; }
    
    /// <summary>
    /// A link to the Web API endpoint providing full details of the artist.
    /// </summary>
    public string Href { get; set; }

    /// <summary>
    /// The Spotify ID for the artist.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The name of the artist
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The object type: "artist"
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The Spotify URI for the artist.
    /// </summary>
    public string Uri { get; set; }
  }
}
