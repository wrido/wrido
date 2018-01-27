using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Hosting;
using Wrido.Electron.Windows;
using Wrido.Logging;

namespace Wrido.Electron
{
  public interface ITrayIconManager
  {
    Task InitAsync(CancellationToken ct = default);
  }

  public class TrayIconManager : ITrayIconManager, IDisposable
  {
    private readonly ILogger _logger;
    private readonly IApplicationLifetime _lifetime;
    private readonly IWindowManager _windowManager;
    private readonly Tray _tray;

    public TrayIconManager(ILogger logger, IApplicationLifetime lifetime, IWindowManager windowManager)
    {
      _logger = logger;
      _lifetime = lifetime;
      _windowManager = windowManager;
      _tray = ElectronNET.API.Electron.Tray;
    }

    public Task InitAsync(CancellationToken ct = default)
    {
      using (_logger.Timed("Initialize Tray icon"))
      {
        _tray.SetTitle("Wrido");
        _tray.DisplayBalloon(new DisplayBalloonOptions
        {
          Title = "Update available",
          Content = "Click here to update to 0.1"
        });
        _tray.Show("./Electron/wrido-64x64.png", new[]
        {
          new MenuItem { Label = "Show...", Click = () => _windowManager.ShowAsync(MainWindow.WindowName, ct)},
          new MenuItem { Label = "About Wrido", Click = () => _windowManager.ShowAsync(AboutWindow.WindowName, ct)},
          new MenuItem { Label = "Exit", Click = () => _lifetime.StopApplication()}
        });

        return Task.CompletedTask;
      }
    }

    public void Dispose()
    {
      _logger.Debug("Disposing System Tray.");
      _tray.Destroy();
    }
  }
}
