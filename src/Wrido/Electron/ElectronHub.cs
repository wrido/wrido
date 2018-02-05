using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Wrido.Electron.Windows;

namespace Wrido.Electron
{
  public class ElectronHub : Hub
  {
    private readonly IWindowManager _windowManager;

    public ElectronHub(IWindowManager windowManager)
    {
      _windowManager = windowManager;
    }

    public Task HideWindowAsync() => _windowManager.HideAsync(MainWindow.WindowName);
  }
}
