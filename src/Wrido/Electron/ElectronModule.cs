using Autofac;
using ElectronNET.API;
using Wrido.Electron.Windows;
using Shell = Wrido.Electron.Windows.Shell;

namespace Wrido.Electron
{
  public class ElectronModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      if (!HybridSupport.IsElectronActive)
      {
        builder.RegisterModule<FakeElectronModule>();
        return;
      }

      builder
        .RegisterType<ElectronHost>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<WindowsServices>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<TrayIconManager>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<ShortcutManager>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<Shell>()
        .AsSelf()
        .SingleInstance();

      builder
        .RegisterType<About>()
        .AsSelf()
        .SingleInstance();
    }
  }
}
