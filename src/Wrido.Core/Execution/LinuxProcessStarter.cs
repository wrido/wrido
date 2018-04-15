using System.Diagnostics;

namespace Wrido.Execution
{
  public class LinuxProcessStarter : IProcessStarter
  {
    public void OpenDefault(string filePath) => StartNewProcess(filePath);

    public void OpenApplication(string applicationName, string arguments) =>
      StartNewProcess($"{applicationName} {arguments}");

    private static void StartNewProcess(string processSpecification)
    {
      Process.Start("xdg-open", processSpecification);
    }
  }
}