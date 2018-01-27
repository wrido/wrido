using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Serilog;

namespace Wrido.Electron.Windows
{
  public class MainWindow : WindowBase
  {
    public const string WindowName = "Main";

    public override string Name => WindowName;
    protected override string Url => "http://localhost";
    private readonly ILogger _logger = Log.ForContext<MainWindow>();

    protected override BrowserWindowOptions Options => new BrowserWindowOptions
    {
      Show = true,
      Title = "Wrido",
      Center = true,
      SkipTaskbar = true,
      Frame = false,
      AutoHideMenuBar = true,
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
  }
}
