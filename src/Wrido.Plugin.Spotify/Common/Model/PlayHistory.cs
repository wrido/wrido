using System;
using Wrido.Plugin.Spotify.Common.Model.Simplified;

namespace Wrido.Plugin.Spotify.Common.Model
{
  public class PlayHistory
  {
    /// <summary>
    /// The track the user listened to.
    /// </summary>
    public Track Track { get; set; }

    /// <summary>
    /// The date and time the track was played.
    /// </summary>
    public DateTime PlayedAt { get; set; }

    /// <summary>
    /// The context the track was played from.
    /// </summary>
    public PlayContext Context { get; set; }
  }
}
