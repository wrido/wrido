using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API;
using Wrido.Configuration;

namespace Wrido.Electron
{
  public interface IElectronService
  {
    Task InitAsync(CancellationToken ct = default);
  }

  public class ShortcutManager : IElectronService, IDisposable
  {
    private readonly IConfigurationProvider _configProvider;
    private readonly IConfigurationFileWatcher _configWatcher;
    private readonly IWindowsServices _windowServices;
    private readonly GlobalShortcut _shortcuts;
    private IAppConfiguration _currentConfig;

    public ShortcutManager(IConfigurationProvider configProvider, IConfigurationFileWatcher configWatcher, IWindowsServices windowServices)
    {
      _configProvider = configProvider;
      _currentConfig = _configProvider.GetAppConfiguration();
      _configWatcher = configWatcher;
      _windowServices = windowServices;
      _shortcuts = ElectronNET.API.Electron.GlobalShortcut;
      configWatcher.Updated += BindShortcuts;
    }

    public Task InitAsync(CancellationToken ct = default)
    {
      BindShortcuts();
      return Task.CompletedTask;
    }

    private void BindShortcuts(object sender, FileSystemEventArgs e) => BindShortcuts();
    private void BindShortcuts()
    {
      var oldConfig = Interlocked.Exchange(ref _currentConfig, _configProvider.GetAppConfiguration());
      if(oldConfig != null)
      {
        _shortcuts.Unregister(oldConfig.HotKey);
      }
      _shortcuts.Register(_currentConfig.HotKey, () => _windowServices.ToggleShellVisibilityAsync());
    }

    public void UnbindShortcuts()
    {
      if (_currentConfig != null)
      {
        _shortcuts.Unregister(_currentConfig.HotKey);
      }
    }

    public void Dispose()
    {
      _configWatcher.Updated -= BindShortcuts;
      _shortcuts.UnregisterAll();
    }
  }
}
