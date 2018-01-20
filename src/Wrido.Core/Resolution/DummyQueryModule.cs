using System;
using Autofac;

namespace Wrido.Core.Resolution
{
  public class DummyQueryModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      var slowProvider = new DummyProvider(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(2), "Slow Provider");
      var fastProvider = new DummyProvider(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(400), "Fast Provider");

      builder
        .RegisterInstance(slowProvider)
        .As<IQueryProvider>();


      builder
        .RegisterInstance(fastProvider)
        .As<IQueryProvider>();
    }
  }
}
