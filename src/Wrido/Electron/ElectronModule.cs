using Autofac;
using ElectronNET.API;
using Wrido.Electron.Windows;

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
        .RegisterType<WindowManager>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<TrayIconManager>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .Register(context => ElectronNET.API.Electron.WindowManager)
        .As<ElectronNET.API.WindowManager>();

      builder
        .RegisterType<MainWindow>()
        .As<WindowBase>()
        .SingleInstance();

      builder
        .RegisterType<AboutWindow>()
        .As<WindowBase>()
        .SingleInstance();
    }
  }
}
