using System;
using System.Collections.Generic;
using Autofac;

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
        .RegisterType<ConfigurationFileWatcher>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder.Register(context => context.Resolve<IConfigurationProvider>().GetAppConfiguration())
        .As<IAppConfiguration>();
    }
  }
}
