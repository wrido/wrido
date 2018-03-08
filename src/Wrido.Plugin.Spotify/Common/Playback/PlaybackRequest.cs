using System.Collections.Generic;

namespace Wrido.Plugin.Spotify.Common.Playback
{
  public class PlaybackRequest
  {
    /// <summary>
    /// Optional. The id of the device this command is targeting.
    /// If not supplied, the user's currently active device is the target.
    /// </summary>
    public string DeviceId { get; set; }

    /// <summary>
    /// Optional. The id of the device this command is targeting.
    /// If not supplied, the user's currently active device is the target.
    /// </summary>
    public string ContextUri { get; set; }

    /// <summary>
    /// Optional. A JSON array of the Spotify track URIs to play.
    /// </summary>
    public IEnumerable<string> Uris { get; set; }

    /// <summary>
    /// Optional. Indicates from where in the context playback should start.
    /// Only available when context_uri corresponds to an album or playlist object, or when the uris parameter is used.
    /// </summary>
    public PlaybackContextOffset Offset { get; set; }
  }

  public abstract class PlaybackContextOffset { }

  public class PositionOffset : PlaybackContextOffset
  {
    public ushort Position { get; set; }

    public PositionOffset(ushort position)
    {
      Position = position;
    }
  }

  public class UriOffset : PlaybackContextOffset
  {
    public string Uri { get; set; }

    public UriOffset(string uri)
    {
      Uri = uri;
    }
  }
}
