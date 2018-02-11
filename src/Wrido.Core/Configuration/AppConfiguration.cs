using System;

namespace Wrido.Configuration
{
  public interface IAppConfiguration
  {
    string HotKey { get; }
    string ConfigurationFilePath { get; }
    string InstallDirectory { get; }
  }

  internal class AppConfiguration : IAppConfiguration
  {
    public string HotKey { get; set; }

    public string ConfigurationFilePath => ReadOnlyAppConfiguration.ConfigurationFilePath;
    public string InstallDirectory => ReadOnlyAppConfiguration.InstallDirectory;
  }

  internal static class ReadOnlyAppConfiguration
  {
    public static string ConfigurationFilePath => $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\herehere.json";
    public static string InstallDirectory => AppDomain.CurrentDomain.BaseDirectory;
  }
}
