namespace Wrido.Configuration
{
  public interface IConfigurationProvider
  {
    IAppConfiguration GetAppConfiguration();
  }

  public class ConfigurationProvider : IConfigurationProvider
  {
    public IAppConfiguration GetAppConfiguration()
    {
      return new AppConfiguration
      {
        HotKey = "CommandOrControl+X"
      };
    }
  }
}
