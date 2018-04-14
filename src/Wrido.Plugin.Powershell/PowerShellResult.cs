using System;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido.Plugin.Powershell
{
  public class PowerShellResult : QueryResult
  {
    private static readonly Image _icon = new Image
    {
      Uri = new Uri("/resources/wrido/plugin/powershell/resources/powershell.png", UriKind.Relative),
      Alt = "PowerShell"
    };

    public bool RunInExternalShell { get; set; }

    public PowerShellResult()
    {
      Icon = _icon;
    }
  }

  public class OpenPowerShellResult : PowerShellResult { }

  public class PowerShellCommandResult : PowerShellResult
  {
    public string Command { get; set; }
    public bool IsValid { get; set; }
  }

  public class PowerShellFileResult : PowerShellResult
  {
    public string FilePath { get; set; }
    public string Arguments { get; set; }
  }
}
