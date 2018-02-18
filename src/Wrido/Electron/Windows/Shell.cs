using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Serilog;

namespace Wrido.Electron.Windows
{
  public class Shell : WindowBase
  {
    public const string WindowName = "Main";

    public override string Name => WindowName;
    protected override string Url => "http://localhost";
    private readonly ILogger _logger = Log.ForContext<Shell>();
    private ShellSize _size;
    private const int _windowWidth = 700;
    private const int _inputWindowHeight = 42;
    private const int _inputAndResultWindowHeight = 450;

    public Shell()
    {
      _size = ShellSize.InputFieldOnly;
    }

    protected override BrowserWindowOptions Options => new BrowserWindowOptions
    {
      Show = true,
      Title = "Wrido",
      Center = true,
      SkipTaskbar = true,
      Frame = false,
      AutoHideMenuBar = true,
      Width = _windowWidth,
      Height = _inputWindowHeight,
      TitleBarStyle = TitleBarStyle.hidden
    };

    protected override Task OnCreatedAsync(BrowserWindow window, CancellationToken ct)
    {
      _logger.Verbose("Registering OnBlur event handler");
      window.OnBlur += () =>
      {
        _logger.Debug("Shell is blured, hiding it");
        window.Hide();
      };
      _logger.Verbose("Registering OnReadyToShow event handler");
      window.OnReadyToShow += () =>
      {
        _logger.Information("Shell is ready to be shown");
        window.Show();
      };
      return Task.CompletedTask;
    }

    public Task ResizeAsync(ShellSize size, CancellationToken ct = default)
    {
      if (size == _size)
      {
        return Task.CompletedTask;
      }
      _size = size;
      var height = size == ShellSize.InputFieldOnly ? _inputWindowHeight : _inputAndResultWindowHeight;
      Window.SetSize(_windowWidth, height);
      return Task.CompletedTask;
    }
  }

  public enum ShellSize
  {
    InputFieldOnly,
    InputAndResults
  }
}
