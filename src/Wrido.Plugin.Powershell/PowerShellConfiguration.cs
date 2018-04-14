using System.Collections.Generic;
using Wrido.Configuration;

namespace Wrido.Plugin.Powershell
{
  public class PowerShellConfiguration : IPluginConfiguration
  {
    public string Keyword {get; set;}
    public IList<RegisteredScript> Scripts { get; set; }

    public static PowerShellConfiguration Default => new PowerShellConfiguration
    {
      Keyword = ":ps"
    };

    public class RegisteredScript
    {
      public string FilePath { get; set; }
      public string Arguments { get; set; }
      public string Alias { get; set; }
      public bool UseExternalShell { get; set; }

      public RegisteredScript()
      {
        Arguments = string.Empty;
        UseExternalShell = true;
      }
    }
  }
}
