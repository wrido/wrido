using System;
using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Serilog;
using Wrido.Logging;

namespace Wrido.Electron.Windows
{
  public abstract class WindowBase : IDisposable
  {
    public abstract string Name { get; }
    public Guid Id { get; }
    public BrowserWindow Window { private set; get; }
    protected abstract BrowserWindowOptions Options { get; }
    protected abstract string Url { get; }
    private bool _isDisposing;
    private readonly Logging.ILogger _logger = new SerilogLogger(Log.ForContext<WindowBase>());

    protected WindowBase()
    {
      Id = Guid.NewGuid();
    }

    public async Task InitAsync(CancellationToken ct)
    {
      _logger.Verbose("Preparing to init {windowName} window (id: {windowId}) with url {windowUrl}", Name, Id, Url);
      using (_logger.Timed("Initiating {windowName} window", Name))
      {
        Window = await ElectronNET.API.Electron.WindowManager.CreateWindowAsync(Options, Url);
        await OnCreatedAsync(Window, ct);
        Window.OnClosed += () =>
        {
          _logger.Debug("Window {windowName} has been closed.", Name);
          if (_isDisposing)
          {
            _logger.Information("Disposing {windowName}. Window wont be recreated", Name);
            return;
          }
          _logger.Information("Recreating window {windowName}", Name);
          InitAsync(CancellationToken.None).GetAwaiter().GetResult();
        };
      }
    }

    protected virtual Task OnCreatedAsync(BrowserWindow window, CancellationToken ct)
    {
      return Task.CompletedTask;
    }

    public void Dispose()
    {
      _isDisposing = true;
    }
  }
}