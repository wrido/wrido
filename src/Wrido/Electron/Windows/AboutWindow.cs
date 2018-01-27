using System.Threading;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace Wrido.Electron.Windows
{
  public class AboutWindow : WindowBase
  {
    public const string WindowName = "About";

    public override string Name => WindowName;

    protected override BrowserWindowOptions Options => new BrowserWindowOptions
    {
      Show = false,
      Title = "About Wrido",
      Center = true,
      SkipTaskbar = true,
      Frame = false,
      AutoHideMenuBar = true,
      TitleBarStyle = TitleBarStyle.hidden
    };

    protected override Task OnCreatedAsync(BrowserWindow window, CancellationToken ct)
    {
      window.OnShow += window.Focus;
      window.OnBlur += window.Hide;
      return Task.CompletedTask;
    }

    protected override string Url => $"http://localhost:{BridgeSettings.WebPort}/about.htm";
  }
}
