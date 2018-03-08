using System.Net;

namespace Wrido.Plugin.Spotify.Common.Model
{
  public class Error
  {
    public HttpStatusCode Status { get; set; }
    public string Message { get; set; }
  }

  public class UnsuccessfulOperation
  {
    public Error Error { get; set; }
  }
}
