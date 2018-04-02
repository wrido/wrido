using System;
using System.Collections.Generic;

namespace Wrido.Plugin.Spotify.Common.Model.Full
{
  public class Track
  {
    /// <summary>
    /// The album on which the track appears. The album object includes a link in href to full information about the album. 
    /// </summary>
    public Simplified.Album Album { get; set; }

    /// <summary>
    /// The artists who performed the track. Each artist object includes a link in href to more detailed information about the artist. 
    /// </summary>
    public IList<Simplified.Artist> Artists { get; set; }

    /// <summary>
    /// A list of the countries in which the track can be played, identified by their ISO 3166-1 alpha-2 code. 
    /// </summary>
    public IList<string> AvailableMarkets { get; set; }

    /// <summary>
    /// The disc number(usually 1 unless the album consists of more than one disc). 
    /// </summary>
    public int DiskNumber { get; set; }

    /// <summary>
    /// The track length derived from milliseconds.
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Whether or not the track has explicit lyrics(true = yes it does; false = no it does not OR unknown). 
    /// </summary>
    public bool Explicit { get; set; }

    /// <summary>
    /// Known external IDs for the track.
    /// </summary>
    public IDictionary<string, string> ExternalIds { get; set; }

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
    /// Part of the response when Track Relinking is applied. If true, the track is playable in the given market. Otherwise false.
    /// </summary>
    public bool IsPlayable { get; set; }

    /// <summary>
    /// Part of the response when Track Relinking is applied, and the requested track has been replaced with different track.
    /// The track in the linked_from object contains information about the originally requested track.
    /// </summary>
    public TrackLink LinkedFrom { get; set; }

    /// <summary>
    /// Part of the response when Track Relinking is applied, the original track is not available in the given market,
    /// and Spotify did not have any tracks to relink it with. The track response will still contain metadata for the original track,
    /// and a restrictions object containing the reason why the track is not available: "restrictions" : {"reason" : "market"}
    /// </summary>
    public string Restrictions { get; set; }

    /// <summary>
    /// The name of the track.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The popularity of the track.The value will be between 0 and 100, with 100 being the most popular.
    /// </summary>
    public int Popularity { get; set; }

    /// <summary>
    /// A link to a 30 second preview (MP3 format) of the track. null if not available.
    /// </summary>
    public Uri PreviewUrl { get; set; }

    /// <summary>
    /// The number of the track.If an album has several discs, the track number is the number on the specified disc.
    /// </summary>
    public int TrackNumber { get; set; }

    /// <summary>
    /// The object type: "track".
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 	The Spotify URI for the track.
    /// </summary>
    public string Uri { get; set; }
  }
}
