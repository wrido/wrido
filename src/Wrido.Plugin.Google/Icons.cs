using System;
using Wrido.Resources;

namespace Wrido.Plugin.Google
{
  public class Icons
  {
    public static readonly Image GoogleLogo = new Image
    {
      Uri = new Uri("/resources/wrido/plugin/google/resources/google.png", UriKind.Relative),
      Alt = "Google"
    };

    public static readonly Image LinkIcon = new Image
    {
      Uri = new Uri("/resources/wrido/plugin/google/resources/link.png", UriKind.Relative),
      Alt = "Google"
    };
  }
}
