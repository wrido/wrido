using System.Management.Automation;
using Autofac;
using Wrido.Configuration;

namespace Wrido.Plugin.Powershell
{
  public class PowerShellPlugin : IWridoPlugin
  {
    public void Register(ContainerBuilder builder)
    {
      builder
        .Register(context => context
          .Resolve<IConfigurationProvider>()
          .GetConfiguration<PowerShellConfiguration>() ?? PowerShellConfiguration.Default)
        .AsSelf()
        .InstancePerDependency();

      builder
        .RegisterType<PowerShellProvider>()
        .AsImplementedInterfaces()
        .InstancePerDependency();

      builder
        .RegisterType<PowerShellExecutor>()
        .AsImplementedInterfaces()
        .InstancePerDependency();
    }
  }
}
