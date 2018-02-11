using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using Wrido.Configuration;
using Wrido.Logging;

namespace Wrido.Plugin
{
  public class NugetPluginLoader : IPluginLoader
  {
    private readonly AssemblyPluginLoader _pluginLoder;
    private readonly IAppConfiguration _config;
    private readonly ILogger _logger;

    public NugetPluginLoader(AssemblyPluginLoader pluginLoder, IAppConfiguration config, ILogger logger)
    {
      _pluginLoder = pluginLoder;
      _config = config;
      _logger = logger;
    }

    public bool TryLoad(string pluginName, out IWridoPlugin plugin)
    {
      if (!pluginName.StartsWith("wrido.plugin", StringComparison.InvariantCultureIgnoreCase))
      {
        _logger.Warning("{pluginName} is not a recognized plugin name", pluginName);
        plugin = default;
        return false;
      }

      if (_pluginLoder.TryLoad(pluginName, out plugin))
      {
        _logger.Verbose("Plugin {pluginName} found by assembly loader", pluginName);
        return true;
      }

      var workingDirectory = CreateWorkingDirectory();
      var downloaded = TryDownloadNuget(workingDirectory, pluginName, out var nugetFile);
      if (!downloaded)
      {
        Directory.Delete(workingDirectory, true);
        _logger.Warning("Unable to load plugin {pluginName}", pluginName);
        return false;
      }

      var nugetDirectory = Path.Combine(workingDirectory, pluginName);
      try
      {
        _logger.Debug("Extracting {nugetFile} to {nugetDirectory}", nugetFile, nugetDirectory);
        ZipFile.ExtractToDirectory(nugetFile, nugetDirectory);
      }
      catch (Exception e)
      {
        Directory.Delete(workingDirectory, true);
        _logger.Warning(e, "Unable to extract {nugetFile} to {nugetDirectory}", nugetFile, nugetDirectory);
        return false;
      }

      var dllFiles = Directory.GetFiles(nugetDirectory, "*.dll", SearchOption.AllDirectories).ToList();
      var pluginDllPath = FindCompatibleDll(dllFiles);
      if (string.IsNullOrWhiteSpace(pluginDllPath))
      {
        Directory.Delete(workingDirectory, true);
        _logger.Warning("Unable to find compatable version of {pluginName}", pluginName);
        return false;
      }

      var targetDllPath = Path.Combine(_config.InstallDirectory, Path.GetFileName(pluginDllPath));
      _logger.Debug("Copying file {pluginDllPath} to {targetDllPath}", pluginDllPath, targetDllPath);
      File.Copy(pluginDllPath, targetDllPath);
      Directory.Delete(workingDirectory, true);
      return _pluginLoder.TryLoad(pluginName, out plugin);
    }

    private string FindCompatibleDll(List<string> dllPaths)
    {
      const string netStandard = "netstandard";
      var compatible = dllPaths.LastOrDefault(p => p.Contains(netStandard));
      if (!string.IsNullOrEmpty(compatible))
      {
        _logger.Debug("Found netstandard dll {dllPath} that is expected to be compatable", compatible);
        return compatible;
      }

      _logger.Information("No netstandard dll found, returning first dll and hope it works");
      return dllPaths.FirstOrDefault();
    }

    private bool TryDownloadNuget(string workingDirectory, string nugetPackageName, out string fileName)
    {
      var nugetUrl = $"https://www.nuget.org/api/v2/package/{nugetPackageName}";
      fileName = Path.Combine(workingDirectory, $"{nugetPackageName}.nupkg");
      _logger.Verbose("Setting download path to {nugetDirectory}", fileName);

      using (var webClient = new WebClient())
      {
        try
        {
          using (_logger.Timed("Download package {pluginName} from NuGet", nugetPackageName))
          {
            webClient.DownloadFile(nugetUrl, fileName);
          }
        }
        catch (Exception e)
        {
          _logger.Information(e, "Unable to download {pluginName} from NuGet.org", nugetPackageName);
          return false;
        }
      }
      return true;
    }

    private string CreateWorkingDirectory()
    {
      var workingDir = Path.Combine(_config.InstallDirectory, Guid.NewGuid().ToString());
      Directory.CreateDirectory(workingDir);
      _logger.Debug("Working directory {workingDirectory} created.", workingDir);
      return workingDir;
    }
  }
}
