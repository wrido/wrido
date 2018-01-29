using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Wrido.Queries
{
  public interface IResultExecuter
  {
    bool CanExecute(QueryResult result);
    Task ExecuteAsync(QueryResult result);
  }

  public class WebResultExecuter : IResultExecuter
  {
    public bool CanExecute(QueryResult result)
    {
      return result is WebResult;
    }

    public Task ExecuteAsync(QueryResult result)
    {
      if(result is WebResult webResult)
      {
        OpenBrowser(webResult.Uri.AbsoluteUri);
        return Task.CompletedTask;
      }
      return Task.FromCanceled(CancellationToken.None);
    }

    public static void OpenBrowser(string url)
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
