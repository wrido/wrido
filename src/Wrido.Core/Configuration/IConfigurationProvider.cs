using System;
using System.Collections.Generic;

namespace Wrido.Configuration
{
  public interface IConfigurationProvider
  {
    IAppConfiguration GetAppConfiguration();
    bool TryGetConfiguration<TPlugin>(string pluginName, out TPlugin pluginConfig) where TPlugin : IPluginConfiguration;
    IEnumerable<string> GetPluginNames();
    event EventHandler ConfigurationUpdated;
  }
}