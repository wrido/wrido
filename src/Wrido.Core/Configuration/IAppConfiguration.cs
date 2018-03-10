using System;

namespace Wrido.Configuration
{
  public interface IAppConfiguration
  {
    string HotKey { get; }
    string ConfigurationFilePath { get; }
    string InstallDirectory { get; }
    Uri ServerUrl { get; }
  }
}