using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Queries;

namespace Wrido.Plugin.Powershell
{
  public class PowerShellProvider : QueryProvider
  {
    private readonly PowerShellConfiguration _config;

    public PowerShellProvider(PowerShellConfiguration config)
    {
      _config = config;
    }

    public override bool CanHandle(Query query) =>
      string.Equals(query.Command, _config.Keyword, StringComparison.CurrentCultureIgnoreCase);

    protected override Task QueryAsync(Query query, CancellationToken ct)
    {
      if (string.IsNullOrWhiteSpace(query.Argument))
      {
        Available(new OpenPowerShellResult
        {
          Title = "Open PowerShell",
          Description = "Create a new PowerShell session",
          RunInExternalShell = true
        });
        return Task.CompletedTask;
      }

      var registedScript = _config.Scripts.FirstOrDefault(s => string.Equals(s.Alias, query.Argument.Trim()));
      if (registedScript != null)
      {
        Available(new PowerShellFileResult
        {
          Title = $"Run {Path.GetFileName(registedScript.FilePath)}",
          Description = $"With arguments {registedScript.Arguments}",
          FilePath = registedScript.FilePath,
          Arguments = registedScript.Arguments,
          RunInExternalShell = registedScript.UseExternalShell
        });
        return Task.CompletedTask;
      }

      Available(new PowerShellCommandResult
      {
        Title = "Execute command in new shell",
        Description = $"{query.Argument}",
        Command = query.Argument,
        RunInExternalShell = true
      });

      Available(new PowerShellCommandResult
      {
        Title = "Execute command in background",
        Description = $"{query.Argument}",
        Command = query.Argument,
        RunInExternalShell = false
      });

      return Task.CompletedTask;
    }
  }
}
