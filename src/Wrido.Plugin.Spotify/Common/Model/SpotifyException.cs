using System;

namespace Wrido.Plugin.Spotify.Common.Model
{
  public class SpotifyException : Exception
  {
    public Error Error { get; }

    public SpotifyException(string message, Error error) :base(message)
    {
      Error = error;
    }
  }
}
