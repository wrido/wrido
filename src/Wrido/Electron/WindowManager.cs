using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API;
using Wrido.Configuration;
using Wrido.Electron.Windows;
using Wrido.Logging;

namespace Wrido.Electron
{
  public interface IWindowManager
  {
    Task InitAsync(CancellationToken ct = default);
    Task ShowAsync(string windowName, CancellationToken ct = default);
    Task HideAsync(string windowName, CancellationToken ct = default);
  }

  public class WindowManager : IDisposable, IWindowManager
  {
    private readonly IAppConfiguration _appConfig;
    private readonly ILogger _logger;
    private readonly GlobalShortcut _shortcuts;
    private readonly Dictionary<string, WindowBase> _windows;

    public WindowManager(IAppConfiguration appConfig, IEnumerable<WindowBase> windows, ILogger logger)
    {
      _appConfig = appConfig;
      _logger = logger;
      _shortcuts = ElectronNET.API.Electron.GlobalShortcut;
      _windows = windows.ToDictionary(w => w.Name, w => w, StringComparer.InvariantCultureIgnoreCase);
    }

    public async Task InitAsync(CancellationToken ct = default)
    {
      using (_logger.Timed("Init Electron"))
      {
        foreach (var window in _windows.Values)
        {
          await window.InitAsync(ct);
        }

        if (!_windows.ContainsKey(MainWindow.WindowName))
        {
          _logger.Warning("No window named {windowName} found. Can not register shortcut");
        }
        else
        {
          var mainWindow = _windows[MainWindow.WindowName];
          _logger.Information("Registering {hotKey} as application hot key", _appConfig.HotKey);
          _shortcuts.Register(_appConfig.HotKey, async () =>
          {
            _logger.Debug("The application hot key is pressed");
            var visible = await mainWindow.Window.IsVisibleAsync();
            if (visible)
            {
              mainWindow.Window.Hide();
            }
            else
            {
              mainWindow.Window.Show();
            }
          });
        }
      }
    }

    public async Task ShowAsync(string windowName, CancellationToken ct = default)
    {
      var showOperation = _logger.Timed("Show window {windowName}", windowName);
      if (!_windows.ContainsKey(windowName))
      {
        _logger.Warning("Unable to find window named {windowName}", windowName);
        showOperation.Cancel();
        return;
      }
      var window = _windows[windowName];
      var isVisible = await window.Window.IsVisibleAsync();
      if (isVisible)
      {
        _logger.Debug("Window {windowName} is already visible.", windowName);
        showOperation.Cancel();
        return;
      }
      _logger.Debug("Window {windowName} (id: {windowId}) is hidden. Showing it", windowName, window.Id);
      window.Window.Show();
      showOperation.Complete();
    }

    public async Task HideAsync(string windowName, CancellationToken ct = default)
    {
      var showOperation = _logger.Timed("Hide window {windowName}", windowName);
      if (!_windows.ContainsKey(windowName))
      {
        _logger.Warning("Unable to find window named {windowName}", windowName);
        showOperation.Cancel();
        return;
      }
      var window = _windows[windowName];
      var isVisible = await window.Window.IsVisibleAsync();
      if (!isVisible)
      {
        _logger.Debug("Window {windowName} is already hidden.", windowName);
        showOperation.Cancel();
        return;
      }
      _logger.Debug("Window {windowName} is visible. Hiding it");
      window.Window.Show();
      showOperation.Complete();
    }

    public void Dispose()
    {
      _logger.Information("Disposing");
      foreach (var window in _windows.Values)
      {
        window.Dispose();
      }
      _shortcuts.Unregister(_appConfig.HotKey);
    }
  }
}
