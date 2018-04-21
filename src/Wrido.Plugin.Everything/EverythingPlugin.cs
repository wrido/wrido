using Autofac;
using Everything;
using Wrido.Configuration;

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

      builder
        .RegisterType<CategoryProvider>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .Register(context =>
        {
          var configProvider = context.Resolve<IConfigurationProvider>();
          return configProvider.GetConfiguration<EverythingConfiguration>() ?? EverythingConfiguration.Default;
        })
        .As<EverythingConfiguration>()
        .InstancePerDependency();
    }
  }
}
