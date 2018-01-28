using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Wrido.Logging;

namespace Wrido.Plugin
{
  public interface IPluginLoader
  {
    bool TryLoad(string pluginName, out IWridoPlugin plugin);
  }

  public class AssemblyPluginLoader : IPluginLoader
  {
    private readonly string _currentDirectory;
    private readonly ILogger _logger;

    public AssemblyPluginLoader(ILogger logger)
    {
      _logger = logger;
      _currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
    }

    public bool TryLoad(string pluginName, out IWridoPlugin plugin)
    {
      plugin = default;

      var assemblyFiles = Directory
        .GetFiles(_currentDirectory)
        .Where(f => f.EndsWith(".dll", StringComparison.OrdinalIgnoreCase));

      var exactMatch = assemblyFiles.FirstOrDefault(f => f.EndsWith($"{pluginName}.dll", StringComparison.InvariantCultureIgnoreCase));
      if (exactMatch == null)
      {
        return false;
      }

      try
      {
        var pluginAssembly = Assembly.LoadFile(exactMatch);
        var pluginTypes = pluginAssembly.DefinedTypes
          .Where(t => t.ImplementedInterfaces.Contains(typeof(IWridoPlugin)))
          .Where(t => t.GetConstructors().Any(c => !c.GetParameters().Any()))
          .ToList();

        if (pluginTypes.Count == 0)
        {
          _logger.Debug("Assembly {assemblyName} did not contain any matching wrido plugins", pluginAssembly.FullName);
          return false;
        }
        if (pluginTypes.Count != 1)
        {
          _logger.Warning("Assembly {assemblyName} contains {pluginCount} plugins. Only one will be loaded. ", pluginAssembly.FullName, pluginTypes.Count);
        }
        var pluginType = pluginTypes.First();
        _logger.Information("Preparing to load plugin {pluginName} of type {pluginType} found in {pluginAssembly}", pluginName, pluginType, pluginAssembly.FullName);
        plugin = (IWridoPlugin) Activator.CreateInstance(pluginType);
        return true;
      }
      catch (Exception e)
      {
        _logger.Warning(e, "Something went wrong when trying to create plugin {pluginName}", pluginName);
        return false;
      }
    }
  }
}
