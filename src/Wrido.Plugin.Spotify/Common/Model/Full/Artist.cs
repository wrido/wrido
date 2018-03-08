using System.Collections.Generic;

namespace Wrido.Plugin.Spotify.Common.Model.Full
{
  public class Artist
  {
    /// <summary>
    /// Known external URLs for this artist.
    /// </summary>
    public IDictionary<string, string> ExtenralUrls { get; set; }

    /// <summary>
    /// Information about the followers of the artist. 
    /// </summary>
    public Followers Followers { get; set; }

    /// <summary>
    /// A list of the genres the artist is associated with. For example:
    /// "Prog Rock", "Post-Grunge". (If not yet classified, the array is empty.) 
    /// </summary>
    public IList<string> Genres { get; set; }

    /// <summary>
    /// A link to the Web API endpoint providing full details of the artist.
    /// </summary>
    public string Href { get; set; }

    /// <summary>
    /// The Spotify ID for the artist.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Images of the artist in various sizes, widest first.
    /// </summary>
    public IList<Image> Images { get; set; }

    /// <summary>
    /// The name of the artist
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The popularity of the artist. The value will be between 0 and 100, with 100 being the most popular.
    /// The artist's popularity is calculated from the popularity of all the artist's tracks.
    /// </summary>
    public int Popularity { get; set; }

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
