using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Wrido.Logging;

namespace Wrido.Electron
{
  public interface IElectronHost
  {
    Task StartAsync(CancellationToken ct = default);
  }

  public class ElectronHost : IElectronHost
  {
    private readonly IWindowManager _windowManager;
    private readonly ITrayIconManager _trayManager;
    private readonly IApplicationLifetime _appLifetime;
    private readonly ILogger _logger;

    public ElectronHost(IWindowManager windowManager, ITrayIconManager trayManager, IApplicationLifetime appLifetime, ILogger logger)
    {
      _windowManager = windowManager;
      _trayManager = trayManager;
      _appLifetime = appLifetime;
      _logger = logger;
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
      _logger.Verbose("Starting electron host");
      var windowTask = _windowManager.InitAsync(ct);
      var trayTask = _trayManager.InitAsync(ct);

      _appLifetime.ApplicationStopping.Register(() =>
      {
        _logger.Information("Application is stopping.");
        (_windowManager as IDisposable)?.Dispose();
        (_trayManager as IDisposable)?.Dispose();
        ElectronNET.API.Electron.App.Quit();
      });

      await Task.WhenAll(windowTask, trayTask);
    }
  }
}
