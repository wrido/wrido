using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Wrido.Execution
{
  public class OpenDefault
  {
    public static void Url(Uri uri) => PathOrUrl(uri.ToString());

    public static void PathOrUrl(string url)
    {
      try
      {
        Process.Start(url);
      }
      catch
      {
        // credit: https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
        // hack because of this: https://github.com/dotnet/corefx/issues/10361
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
          url = url.Replace("&", "^&");
          Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
          Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
          Process.Start("open", url);
        }
        else
        {
          throw;
        }
      }
    }
  }
}
