using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Serilog;
using Wrido.Electron.Windows;

namespace Wrido.Electron
{
  public class FakeElectronModule : Module
  {
    private readonly ILogger _logger = Log.ForContext<FakeElectronModule>();

    protected override void Load(ContainerBuilder builder)
    {
      _logger.Information("Wiring up Fake Electron services");

      builder
        .RegisterType<FakeElectronHost>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<FakeWindowsServices>()
        .AsImplementedInterfaces()
        .SingleInstance();
    }
  }

  public class FakeElectronHost : IElectronHost
  {
    public Task StartAsync(CancellationToken ct = default) => Task.CompletedTask;
  }

  public class FakeWindowsServices : IWindowsServices
  {
    public Task ShowAboutAsync(CancellationToken ct = default) => Task.CompletedTask;
    public Task HideAboutAsync(CancellationToken ct = default) => Task.CompletedTask;
    public Task ShowShellAsync(CancellationToken ct = default) => Task.CompletedTask;
    public Task HideShellAsync(CancellationToken ct = default) => Task.CompletedTask;
    public Task ToggleShellVisibilityAsync(CancellationToken ct = default) => Task.CompletedTask;
    public Task ResizeShellAsync(ShellSize size, CancellationToken ct = default) => Task.CompletedTask;
  }
}
