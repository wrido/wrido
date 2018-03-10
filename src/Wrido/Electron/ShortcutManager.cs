using System;
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
    private readonly IWindowsServices _windowServices;
    private readonly GlobalShortcut _shortcuts;
    private IAppConfiguration _currentConfig;

    public ShortcutManager(IConfigurationProvider configProvider, IWindowsServices windowServices)
    {
      _configProvider = configProvider;
      _currentConfig = _configProvider.GetAppConfiguration();
      _windowServices = windowServices;
      _shortcuts = ElectronNET.API.Electron.GlobalShortcut;
      configProvider.ConfigurationUpdated += (sender, args) => BindShortcuts();
    }

    public Task InitAsync(CancellationToken ct = default)
    {
      BindShortcuts();
      return Task.CompletedTask;
    }

    private void BindShortcuts()
    {
      var oldConfig = Interlocked.Exchange(ref _currentConfig, _configProvider.GetAppConfiguration());
      if(oldConfig != null)
      {
        _shortcuts.Unregister(oldConfig.HotKey);
      }
      _shortcuts.Register(_currentConfig.HotKey, () => _windowServices.ToggleShellVisibilityAsync());
    }

    public void Dispose()
    {
      _shortcuts.UnregisterAll();
    }
  }
}
