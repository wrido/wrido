using Autofac;
using Everything;

namespace Wrido.Plugin.Everything
{
  public class EverythingPlugin : IWridoPlugin
  {
    public void Register(ContainerBuilder builder)
    {
      builder
        .RegisterType<EverythingClient>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<EverythingProvider>()
        .AsImplementedInterfaces()
        .InstancePerDependency();

      builder
        .RegisterType<EverythingExecutor>()
        .AsImplementedInterfaces()
        .InstancePerDependency();
    }
  }
}
