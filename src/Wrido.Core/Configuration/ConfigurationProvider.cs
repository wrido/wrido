using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wrido.Logging;

namespace Wrido.Configuration
{
  public interface IConfigurationProvider
  {
    Task ReloadAsync(CancellationToken ct = default);
    IAppConfiguration GetAppConfiguration();
    bool TryGetConfiguration<TPlugin>(string pluginName, out TPlugin pluginConfig) where TPlugin : IPluginConfiguration;
    IEnumerable<string> GetPluginNames();
  }

  public class ConfigurationProvider : IConfigurationProvider
  {
    private readonly ILogger _logger;
    private readonly string _wridoFolder;
    private readonly string _wridoCfgFile;
    private AppConfiguration _appConfig;
    private IDictionary<string, JToken> _plugins;

    public ConfigurationProvider(ILogger logger)
    {
      _logger = logger;
      _wridoFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\.wrido";
      _wridoCfgFile = $"{_wridoFolder}\\wrido.json";

      LoadConfiguration();
    }

    public Task ReloadAsync(CancellationToken ct = default)
    {
      LoadConfiguration();
      return Task.CompletedTask;
    }

    private void LoadConfiguration()
    {
      var cfgOperation = _logger.Timed("Load application configuration");

      if (!File.Exists(_wridoCfgFile))
      {
        _logger.Information("No config file found at {configPath}. Will default to fallback configuration", _wridoCfgFile);
        _appConfig = new AppConfiguration
        {
          HotKey = "CommandOrControl+X"
        };
        _plugins = new Dictionary<string, JToken>(StringComparer.InvariantCultureIgnoreCase)
        {
          {"Wrido.Plugin.StackExchange", new JValue("Wrido.Plugin.StackExchange")},
          {"Wrido.Plugin.Google", new JValue("Wrido.Plugin.Google")},
          {"Wrido.Plugin.Wikipedia", new JValue("Wrido.Plugin.Wikipedia")},
          {"Wrido.Plugin.Dummy", new JValue("Wrido.Plugin.Dummy")}
        };
        cfgOperation.Cancel();
        return;
      }

      try
      {
        var cfgJson = File.ReadAllText(_wridoCfgFile);
        var dynamicConfig = JObject.Parse(cfgJson);
        _appConfig = dynamicConfig.ToObject<AppConfiguration>();

        var plugins = dynamicConfig.GetValue("plugins", StringComparison.OrdinalIgnoreCase);
        if (plugins == null)
        {
          _logger.Information("Configuration does not contain a 'plugin' section.");
          return;
        }
        if (plugins.Type != JTokenType.Array)
        {
          _logger.Warning("Expected plugin configuration to be array, but received {tokenType}", plugins.Type);
          cfgOperation.Cancel();
          return;
        }
        var pluginsArray = plugins as JArray;
        _plugins = new Dictionary<string, JToken>(StringComparer.InvariantCultureIgnoreCase);
        _logger.Information("{pluginCount} plugin entries in configuration", pluginsArray?.Count ?? 0);
        foreach (var plugin in pluginsArray ?? Enumerable.Empty<JToken>())
        {
          if (plugin.Type == JTokenType.String)
          {
            _plugins.Add(plugin.Value<string>(), plugin);
            continue;
          }
          if (plugin.Type == JTokenType.Object)
          {
            var pluginObj = plugin.Value<JObject>();
            var nameToken = pluginObj.GetValue("Name", StringComparison.OrdinalIgnoreCase);
            if (nameToken == null)
            {
              _logger.Warning("Expected plugin object to have property 'name'.");
              continue;
            }
            if (nameToken.Type != JTokenType.String)
            {
              _logger.Warning("Expected plugin name to be a string, but got {tokenType}", nameToken.Type);
              continue;
            }
            _plugins.Add(nameToken.Value<string>(), pluginObj);
          }
          _logger.Warning("Unidentified plugin of type {tokenType}", plugin.Type);
        }
        cfgOperation.Complete();
      }
      catch (Exception e)
      {
        _logger.Warning(e, "An exception was thrown while deserializing the configuration.");
        cfgOperation.Cancel();
      }
    }

    public IAppConfiguration GetAppConfiguration() => _appConfig;

    public bool TryGetConfiguration<TPlugin>(string pluginName, out TPlugin pluginConfig) where TPlugin : IPluginConfiguration
    {
      pluginConfig = default;
      if (!_plugins.ContainsKey(pluginName))
      {
        return false;
      }
      try
      {
        pluginConfig = _plugins[pluginName].ToObject<TPlugin>();
        return true;
      }
      catch (Exception e)
      {
        _logger.Information(e, "Unable to extract {pluginName} configuration as a {pluginType}", pluginName, typeof(TPlugin).Name);
        return false;
      }
    }

    public IEnumerable<string> GetPluginNames() => _plugins.Keys;
  }
}
