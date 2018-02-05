using System;
using System.Reflection;

namespace Wrido.Configuration
{
  public interface IAppConfiguration
  {
    string HotKey { get; }
    string ConfigurationDirectory { get; }
    string InstallDirectory { get; }
  }

  internal class AppConfiguration : IAppConfiguration
  {
    public string HotKey { get; set; }

    public string ConfigurationDirectory => $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\.wrido\\";
    public string InstallDirectory => AppDomain.CurrentDomain.BaseDirectory;
  }
}
