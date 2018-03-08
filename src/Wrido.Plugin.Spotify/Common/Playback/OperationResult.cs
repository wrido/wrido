namespace Wrido.Plugin.Spotify.Common.Playback
{
  public enum OperationResult
  {
    Unknown,
    Success = 204,
    DeviceUnavailable = 202,
    DeviceNotFound = 404,
    NonPremiumUser = 403,
  }
}
