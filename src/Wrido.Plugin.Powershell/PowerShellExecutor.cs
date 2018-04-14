using System.Management.Automation;
using System.Threading.Tasks;
using Wrido.Execution;
using Wrido.Logging;
using Wrido.Queries;

namespace Wrido.Plugin.Powershell
{
  public class PowerShellExecutor : IResultExecuter
  {
    private readonly ILogger _logger;

    public PowerShellExecutor(ILogger logger)
    {
      _logger = logger;
    }

    public bool CanExecute(QueryResult result) => result is PowerShellResult;

    public Task ExecuteAsync(QueryResult result)
    {
      switch (result)
      {
        case OpenPowerShellResult _:
          OpenDefault.PathOrUrl("powershell -NoExit");
          break;
        case PowerShellFileResult fileResult:
          if (fileResult.RunInExternalShell)
          {
            OpenDefault.PathOrUrl($"powershell -NoExit -File {fileResult.FilePath} {fileResult.Arguments}");
          }
          else
          {
            using (var shellSession = PowerShell.Create())
            {
              shellSession.AddScript("Set-ExecutionPolicy Bypass -Scope Process");
              shellSession.AddScript(fileResult.FilePath);
              shellSession.Invoke();
            }
          }
          break;
        case PowerShellCommandResult powershellResult:
          if (powershellResult.RunInExternalShell)
          {
            OpenDefault.PathOrUrl($"powershell -NoExit -Command {powershellResult.Command}");
          }
          else
          {
            using (var shellSession = PowerShell.Create())
            {
              shellSession.AddScript(powershellResult.Command);
              var executionOutput = shellSession.Invoke(powershellResult.Command);
              if (shellSession.HadErrors)
              {
                _logger.Information("Command {powerShellCommand} had errors", powershellResult.Command);
              }
            }
          }
          break;
      }

      return Task.CompletedTask;
    }
  }
}
