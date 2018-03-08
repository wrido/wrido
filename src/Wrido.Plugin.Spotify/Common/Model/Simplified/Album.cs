using System;
using System.Collections.Generic;

namespace Wrido.Plugin.Spotify.Common.Model.Simplified
{
  public class Album
  {
    /// <summary>
    /// The type of the album: one of "album", "single", or "compilation". 
    /// </summary>
    public AlbumType AlbumType { get; set; }

    /// <summary>
    /// The artists of the album. Each artist object includes a link in href to more detailed information about the artist.
    /// </summary>
    public IList<Artist> Artists { get; set; }

    /// <summary>
    /// 	The markets in which the album is available: ISO 3166-1 alpha-2 country codes.
    /// Note that an album is considered available in a market when at least 1 of its tracks is available in that market.
    /// </summary>
    public IList<string> AvailableMarkets { get; set; }

    /// <summary>
    /// Known external URLs for this album.
    /// </summary>
    public IDictionary<string, Uri> ExternalUrls { get; set; }

    /// <summary>
    /// 	A link to the Web API endpoint providing full details of the album.
    /// </summary>
    public Uri Href { get; set; }

    /// <summary>
    /// The Spotify ID for the album. 
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The cover art for the album in various sizes, widest first.
    /// </summary>
    public IList<Image> Images { get; set; }

    /// <summary>
    /// The name of the album. In case of an album takedown, the value may be an empty string.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The object type: "album"
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The date the album was first released, for example "1981-12-15". Depending on the precision, it might be shown as "1981" or "1981-12".
    /// </summary>
    public string ReleaseDate { get; set; }

    public ReleaseDatePrecision ReleaseDatePrecision { get; set; }

    /// <summary>
    /// The Spotify URI for the album.
    /// </summary>
    public string Uri { get; set; }
  }

  public enum AlbumType
  {
    Album,
    Single,
    Compilation
  }

  public enum ReleaseDatePrecision
  {
    Year,
    Month,
    Day
  }
}
