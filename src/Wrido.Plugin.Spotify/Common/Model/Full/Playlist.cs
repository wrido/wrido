using System;
using System.Collections.Generic;

namespace Wrido.Plugin.Spotify.Common.Model.Full
{
  public class Playlist
  {
    /// <summary>
    /// true if the owner allows other users to modify the playlist. 
    /// </summary>
    public bool Collaborative { get; set; }

    /// <summary>
    /// The playlist description. Only returned for modified, verified playlists, otherwise null.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Known external URLs for this playlist.
    /// </summary>
    public IDictionary<string, Uri> ExternalUrls { get; set; }

    /// <summary>
    /// Information about the followers of the playlist.
    /// </summary>
    public Followers Followers { get; set; }

    /// <summary>
    /// A link to the Web API endpoint providing full details of the playlist.
    /// </summary>
    public Uri Href { get; set; }

    /// <summary>
    /// The Spotify ID for the playlist. 
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Images for the playlist. The array may be empty or contain up to three images. The images are returned by size in descending order. See Working with Playlists.
    ///
    /// Note: If returned, the source URL for the image(url) is temporary and will expire in less than a day.
    /// </summary>
    public IList<Image> Images { get; set; }

    /// <summary>
    /// The name of the playlist.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The user who owns the playlist
    /// </summary>
    public PublicUser Owner { get; set; }

    /// <summary>
    /// The playlist's public/private status: true the playlist is public, false the playlist is private, null the playlist status is not relevant.
    /// </summary>
    public bool Public { get; set; }

    /// <summary>
    /// The version identifier for the current playlist. Can be supplied in other requests to target a specific playlist version: see Remove tracks from a playlist
    /// </summary>
    public string SnapshotId { get; set; }

    /// <summary>
    /// Information about the tracks of the playlist. 
    /// </summary>
    public PaginationResult<PlaylistTrack> Tracks { get; set; }

    /// <summary>
    /// The object type: "playlist"
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The Spotify URI for the playlist.
    /// </summary>
    public string Uri { get; set; }
  }

  public class PlaylistTrack
  {
    /// <summary>
    /// The date and time the track was added.
    /// Note that some very old playlists may return null in this field.
    /// </summary>
    public DateTime? AddedAt { get; set; }

    /// <summary>
    /// The Spotify user who added the track.
    /// Note that some very old playlists may return null in this field.
    /// </summary>
    public PublicUser AddedBy { get; set; }

    /// <summary>
    /// Whether this track is a local file or not.
    /// </summary>
    public bool IsLocal { get; set; }

    /// <summary>
    /// Information about the track.
    /// </summary>
    public Track Track { get; set; }
  }
}
