using System;

namespace Wrido.Configuration
{
  internal class AppConfiguration : IAppConfiguration
  {
    public string HotKey { get; set; }
    public Uri ServerUrl { set; get; }
    public string ConfigurationFilePath => ReadOnlyAppConfiguration.ConfigurationFilePath;
    public string InstallDirectory => ReadOnlyAppConfiguration.InstallDirectory;
  }

  public static class ReadOnlyAppConfiguration
  {
    public static string ConfigurationFilePath => $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\herehere.json";
    public static string ConfigurationLocalFilePath => $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\herehere.local.json";
    public static string InstallDirectory => AppDomain.CurrentDomain.BaseDirectory;
  }
}
