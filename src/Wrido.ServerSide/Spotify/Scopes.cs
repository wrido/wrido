namespace Wrido.ServerSide.Spotify
{
  public class Scopes
  {
    /// <summary>
    /// Read access to user's private playlists.
    /// Prompt shown to user: "Access your private playlists"
    /// </summary>
    public const string PlaylistReadPrivate = "playlist-read-private";

    /// <summary>
    /// Include collaborative playlists when requesting a user's playlists.
    /// Prompt shown to user: "Access your collaborative playlists"
    /// </summary>
    public const string PlaylistReadCollaborative = "playlist-read-collaborative";

    /// <summary>
    /// Write access to a user's public playlists.
    /// Prompt shown to user: "Manage your public playlists"
    /// </summary>
    public const string PlaylistModifyPublic = "playlist-modify-public";

    /// <summary>
    /// Write access to a user's private playlists.
    /// Prompt shown to user: "Manage your private playlists"
    /// </summary>
    public const string PlaylistModifyPrivate = "playlist-modify-private";

    /// <summary>
    /// Control playback of a Spotify track. This scope is currently only available
    /// to Spotify native SDKs (for example, the iOS SDK and the Android SDK).
    /// The user must have a Spotify Premium account.
    /// </summary>
    public const string Streaming = "streaming";

    /// <summary>
    /// Control playback on Spotify clients and Spotify Connect devices.
    /// Prompt shown to user: "Control playback on your Spotify clients and Spotify Connect devices"
    /// </summary>
    public const string UserModifyPlaybackState = "user-modify-playback-state";

    /// <summary>
    /// Read access to a user's currently playing track.
    /// Prompt shown to user: "Read your currently playing track"
    /// </summary>
    public const string UserReadCurrentlyPlaying = "user-read-currently-playing";

    /// <summary>
    /// Read access to a user's playback state.
    /// Prompt shown to user: "Read your currently playing track and Spotify Connect devices information"
    /// </summary>
    public const string UserReadPlaybackState = "user-read-playback-state";

    /// <summary>
    /// Read access to a user's recently played items.
    /// Prompt shown to user: "Access your recently played items"
    /// </summary>
    public const string UserReadRecentlyPlayed = "user-read-recently-played";
  }
}
