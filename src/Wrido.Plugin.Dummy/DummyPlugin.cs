using System;
using Autofac;

namespace Wrido.Plugin.Dummy
{
  public class DummyPlugin : IWridoPlugin
  {
    public void Register(ContainerBuilder builder)
    {
      builder
        .Register(c => new DummyProvider(
          TimeSpan.FromMilliseconds(100),
          TimeSpan.FromSeconds(2),
          "Slow Provider",
          new Uri("/resources/wrido/plugin/dummy/resources/banana.png", UriKind.Relative)))
        .AsImplementedInterfaces()
        .InstancePerDependency();

      builder
        .Register(c => new DummyProvider(
          TimeSpan.FromMilliseconds(100),
          TimeSpan.FromMilliseconds(400),
          "Fast Provider",
          new Uri("/resources/wrido/plugin/dummy/resources/phone.png", UriKind.Relative)))
        .AsImplementedInterfaces()
        .InstancePerDependency();
    }
  }
}
