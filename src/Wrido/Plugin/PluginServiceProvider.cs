using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Wrido.Configuration;
using Wrido.Logging;

namespace Wrido.Plugin
{
  public class PluginServiceProvider : IDisposable
  {
    private readonly IConfigurationProvider _configurationProvider;
    private readonly ILogger _logger;
    private readonly List<IPluginLoader> _pluginLoaders;
    private readonly IContainer _rootPluginContainer;
    private readonly IList<ILifetimeScope> _pluginScopes;

    public PluginServiceProvider(IConfigurationProvider configurationProvider, IEnumerable<IPluginLoader> pluginLoaders, ILogger logger)
    {
      _pluginScopes = new List<ILifetimeScope>();
      _configurationProvider = configurationProvider;
      _logger = logger;
      _pluginLoaders = pluginLoaders.ToList();
      _rootPluginContainer = CreatePluginContainer();
      CreatePluginLifetimeScopes();
    }

    public IContainer CreatePluginContainer()
    {
      _logger.Verbose("Creating Plugin container.");
      var builder = new ContainerBuilder();

      builder
        .RegisterModule<ConfigurationModule>();

      return builder.Build();
    }

    private void CreatePluginLifetimeScopes()
    {
      using (_logger.Timed("Creating lifetime scopes for plugins"))
      {
        foreach (var pluginScope in _pluginScopes)
        {
          _logger.Verbose("Disposing lifetime scope for {pluginName}", pluginScope.Tag);
          pluginScope.Dispose();
        }

        var pluginNames = _configurationProvider.GetPluginNames();
        foreach (var pluginName in pluginNames)
        {
          _logger.Debug("Preparing to load plugin {pluginName}", pluginName);
          foreach (var pluginLoader in _pluginLoaders)
          {
            if (pluginLoader.TryLoad(pluginName, out var plugin))
            {
              _logger.Information("Successfully loaded plugin {pluginName} using {pluginLoader}", pluginName, pluginLoader.GetType().Name);
              _pluginScopes.Add(_rootPluginContainer.BeginLifetimeScope(pluginName, b =>
              {
                plugin.Register(b);
                b.RegisterModule<LoggingModule>();
              }));
              break;
            }
            _logger.Verbose("The plugin loader {pluginLoader} was unable to load {pluginName}", pluginLoader.GetType().Name, pluginName);
          }
        }
      }
    }

    public IEnumerable<TService> GetServices<TService>()
    {
      foreach (var pluginScope in _pluginScopes)
      {
        var pluginServices = pluginScope.Resolve<IEnumerable<TService>>();
        foreach (var service in pluginServices)
        {
          yield return service;
        }
      }
    }

    public void Dispose()
    {
      foreach (var pluginScope in _pluginScopes)
      {
        pluginScope.Dispose();
      }
      _rootPluginContainer.Dispose();
    }
  }
}
