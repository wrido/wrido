using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Wrido.Execution
{
  public class MacProcessStarter : IProcessStarter
  {
    public MacProcessStarter()
    {
      if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
      {
        throw new PlatformNotSupportedException();
      }
    }
    public void OpenDefault(string filePath)
    {
      Process.Start("open", filePath);
    }

    public void OpenApplication(string applicationName, string arguments = default)
    {
      arguments = arguments ?? string.Empty;
      Process.Start("open", $"-a {applicationName} --args {arguments}");
    }
  }
}