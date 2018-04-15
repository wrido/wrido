namespace Wrido.Execution
{
  public interface IProcessStarter
  {
    void OpenDefault(string filePath);
    void OpenApplication(string applicationName, string arguments);
  }
}