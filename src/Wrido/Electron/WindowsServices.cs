using System;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Electron.Windows;

namespace Wrido.Electron
{
  public interface IWindowsServices
  {
    Task ShowAboutAsync(CancellationToken ct = default);
    Task HideAboutAsync(CancellationToken ct = default);
    Task ShowShellAsync(CancellationToken ct = default);
    Task HideShellAsync(CancellationToken ct = default);
    Task ToggleShellVisibilityAsync(CancellationToken ct = default);
    Task ResizeShellAsync(ShellSize size, CancellationToken ct = default);
  }

  public class WindowsServices : IWindowsServices, IElectronService, IDisposable
  {
    private readonly Shell _shell;
    private readonly About _about;

    public WindowsServices(Shell shell, About about)
    {
      _shell = shell;
      _about = about;
    }

    public Task InitAsync(CancellationToken ct)
    {
      return Task.WhenAll(
        _shell.InitAsync(ct),
        _about.InitAsync(ct)
      );
    }

    public Task ShowAboutAsync(CancellationToken ct)
    {
      _about.Window.Show();
      return Task.CompletedTask;
    }

    public Task HideAboutAsync(CancellationToken ct)
    {
      _about.Window.Hide();
      return Task.CompletedTask;
    }

    public Task ShowShellAsync(CancellationToken ct)
    {
      _shell.Window.Show();
      return Task.CompletedTask;
    }

    public Task HideShellAsync(CancellationToken ct)
    {
      _shell.Window.Hide();
      return Task.CompletedTask;
    }

    public async Task ToggleShellVisibilityAsync(CancellationToken ct)
    {
      var isVisible = await _shell.Window.IsVisibleAsync();
      if (isVisible)
      {
        _shell.Window.Hide();
      }
      else
      {
        _shell.Window.Show();
      }
    }

    public Task ResizeShellAsync(ShellSize size, CancellationToken ct)
    {
      return _shell.ResizeAsync(size, ct);
    }

    public void Dispose()
    {
      _shell?.Dispose();
      _about?.Dispose();
    }
  }
}
