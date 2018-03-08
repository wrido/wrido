using System;
using Wrido.Plugin.Spotify.Common.Model;
using Wrido.Plugin.Spotify.Common.Model.Simplified;
using Track = Wrido.Plugin.Spotify.Common.Model.Full.Track;

namespace Wrido.Plugin.Spotify.Common.Playback
{
  public class CurrentPlayback
  {
    /// <summary>
    /// Timestamp when data was fetched
    /// </summary>
    public DateTime TimeStamp { get; set; }

    /// <summary>
    ///  The device that is currently active
    /// </summary>
    public Device Device { get; set; }

    /// <summary>
    /// off, track, context
    /// </summary>
    public RepeatState RepeatState { get; set; }

    /// <summary>
    /// If shuffle is on or off
    /// </summary>
    public bool ShuffleState { get; set; }

    /// <summary>
    /// Progress into the currently playing track. Can be null.
    /// </summary>
    public int? ProgressMs { get; set; }

    /// <summary>
    /// If something is currently playing.
    /// </summary>
    public bool IsPlaying { get; set; }

    /// <summary>
    ///  A Context Object. Can be null.
    /// </summary>
    public PlayContext Context { get; set; }

    /// <summary>
    /// The currently playing track. Can be null.
    /// </summary>
    public Track Item { get; set; }
  }

  public enum RepeatState
  {
    Off,
    Track,
    Context
  }
}
