using Autofac;
using Microsoft.Extensions.Configuration;

namespace Wrido.Configuration
{
  public class ConfigurationModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder
        .RegisterType<ConfigurationProvider>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterInstance(new ConfigurationBuilder()
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile(ReadOnlyAppConfiguration.ConfigurationFilePath, optional: true, reloadOnChange: true)
          .AddJsonFile(ReadOnlyAppConfiguration.ConfigurationLocalFilePath, optional: true, reloadOnChange: true)
          .Build())
        .As<IConfiguration>();

      builder.Register(context => context.Resolve<IConfigurationProvider>().GetAppConfiguration())
        .As<IAppConfiguration>();
    }
  }
}
