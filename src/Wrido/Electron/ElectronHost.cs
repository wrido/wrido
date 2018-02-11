using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly IApplicationLifetime _appLifetime;
    private readonly ILogger _logger;
    private readonly List<IElectronService> _electronServices;

    public ElectronHost(IEnumerable<IElectronService> electronServices, IApplicationLifetime appLifetime, ILogger logger)
    {
      _appLifetime = appLifetime;
      _logger = logger;
      _electronServices = electronServices.ToList();
    }

    public async Task StartAsync(CancellationToken ct = default)
    {
      _logger.Verbose("Starting electron host");

      var initTasks = _electronServices
        .Select(s => s.InitAsync(ct))
        .ToArray();

      _appLifetime.ApplicationStopping.Register(() =>
      {
        _logger.Information("Application is stopping.");
        foreach (var electronService in _electronServices)
        {
          (electronService as IDisposable)?.Dispose();
        }
        ElectronNET.API.Electron.App.Quit();
      });

      await Task.WhenAll(initTasks);
    }
  }
}
