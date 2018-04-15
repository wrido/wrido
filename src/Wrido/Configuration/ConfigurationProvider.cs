using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Wrido.Logging;

namespace Wrido.Configuration
{
  public class ConfigurationProvider : IConfigurationProvider
  {
    private readonly ILogger _logger;
    private readonly IConfiguration _configuration;
    private AppConfiguration _appConfig;
    private Dictionary<string, IConfigurationSection> _plugins;
    private Dictionary<string, object> _pluginCache;

    public ConfigurationProvider(IConfiguration configuration, ILogger logger)
    {
      _logger = logger;
      _configuration = configuration;

      SetupReloadCallback();
      LoadConfiguration();
    }

    private void SetupReloadCallback()
    { 
      ChangeToken.OnChange(_configuration.GetReloadToken, () =>
      {
        // Currently triggers twice, but should be OK for now.
        // More info: https://github.com/aspnet/Home/issues/2542
        LoadConfiguration();
        ConfigurationUpdated?.Invoke(this, new EventArgs());
      });
    }

    private void LoadConfiguration()
    {
      using (_logger.Timed("Load configuration from disk"))
      {
        _appConfig = _configuration.Get<AppConfiguration>();
        _plugins = new Dictionary<string, IConfigurationSection>(StringComparer.InvariantCultureIgnoreCase);
        _pluginCache = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
        var pluginsSection = _configuration.GetSection("plugins");

        if (!pluginsSection.Exists())
        {
          _logger.Information("No plugin sections found, loading some defaults.");
          _plugins.Add("Wrido.Plugin.StackExchange", null);
          _plugins.Add("Wrido.Plugin.Google", null);
          _plugins.Add("Wrido.Plugin.Wikipedia", null);
          _plugins.Add("Wrido.Plugin.Spotify", null);
          _plugins.Add("Wrido.Plugin.Everything", null);
          _plugins.Add("Wrido.Plugin.PowerShell", null);
          return;
        }

        foreach (var pluginSection in pluginsSection.GetChildren())
        {
          if (!string.IsNullOrEmpty(pluginSection.Value))
          {
            if (_plugins.ContainsKey(pluginSection.Value))
            {
              _logger.Warning("A plugin named {pluginName} is already registered. Skipping", pluginSection.Value);
              continue;
            }
            _plugins.Add(pluginSection.Value, default);
          }
          else
          {
            var nameSection = pluginSection.GetSection("name");
            if (string.IsNullOrEmpty(nameSection?.Value))
            {
              _logger.Warning("Plugin {section} does not contain any name section. Skipping.", pluginSection.Key);
              continue;
            }

            if (_plugins.ContainsKey(nameSection.Value))
            {
              _logger.Warning("A plugin named {pluginName} is already registered. Skipping", nameSection.Value);
              continue;
            }
            _plugins.Add(nameSection.Value, pluginSection);
          }
        }
      }
    }

    public IAppConfiguration GetAppConfiguration() => _appConfig;

    public bool TryGetConfiguration<TPlugin>(string pluginName, out TPlugin pluginConfig) where TPlugin : IPluginConfiguration
    {
      var pluginOperation = _logger.Timed("Load plugin configuration: {pluginName}", pluginName);
      pluginConfig = default;

      if (_pluginCache.ContainsKey(pluginName))
      {
        var cachedObj = _pluginCache[pluginName];
        if (cachedObj is TPlugin cachedCfg)
        {
          _logger.Information("Plugin for {pluginName} found in cache", pluginName);
          pluginConfig = cachedCfg;
          pluginOperation.Complete();
          return true;
        }
      }

      if (!_plugins.ContainsKey(pluginName))
      {
        _logger.Information("Plugin {pluginName} not found in configuration.", pluginName);
        pluginOperation.Cancel();
        return false;
      }

      var configSection = _plugins[pluginName];
      pluginConfig = configSection == null
        ? default
        : configSection.Get<TPlugin>();

      var pluginLocalConfig = _configuration.GetSection(pluginName);
      if (pluginLocalConfig == null)
      {
        if (pluginConfig == null)
        {
          pluginOperation.Cancel();
          return false;
        }
        _pluginCache.TryAdd(pluginName, pluginConfig);
        pluginOperation.Complete();
        return true;
      }

      if (pluginConfig == null)
      {
        pluginConfig = pluginLocalConfig.Get<TPlugin>();
      }
      else
      {
        pluginLocalConfig.Bind(pluginConfig);
      }

      if (pluginConfig == null)
      {
        pluginOperation.Cancel();
        return false;
      }
      _pluginCache.Add(pluginName, pluginConfig);
      pluginOperation.Complete();
      return true;
    }

    public IEnumerable<string> GetPluginNames() => _plugins.Keys;

    public event EventHandler ConfigurationUpdated;
  }
}
