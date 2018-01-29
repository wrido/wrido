using System;
using Autofac;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido.Plugin.Dummy
{
  public class DummyPlugin : IWridoPlugin
  {
    public void Register(ContainerBuilder builder)
    {
      var slowProvider = new DummyProvider(
        TimeSpan.FromMilliseconds(100),
        TimeSpan.FromSeconds(2),
        "Slow Provider",
        new Uri("/resources/wrido/plugin/dummy/resources/banana.png", UriKind.Relative));

      var fastProvider = new DummyProvider(
        TimeSpan.FromMilliseconds(100),
        TimeSpan.FromMilliseconds(400),
        "Fast Provider",
        new Uri("/resources/wrido/plugin/dummy/resources/phone.png", UriKind.Relative));

      builder.RegisterResources<DummyPlugin>();

      builder
        .RegisterInstance(slowProvider)
        .As<IQueryProvider>();


      builder
        .RegisterInstance(fastProvider)
        .As<IQueryProvider>();
    }
  }
}
