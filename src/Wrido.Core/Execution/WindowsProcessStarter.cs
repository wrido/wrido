using System.Diagnostics;

namespace Wrido.Execution
{
  public class WindowsProcessStarter : IProcessStarter
  {
    public void OpenDefault(string filePath) => StartNewProcess(filePath);

    public void OpenApplication(string applicationName, string arguments) =>
      StartNewProcess($"{applicationName} {arguments}");

    private static void StartNewProcess(string processSpecification)
    {
      processSpecification = processSpecification.Replace("&", "^&");
      Process.Start(new ProcessStartInfo("cmd", $"/c start {processSpecification}") { CreateNoWindow = true });      
    }
  }
}