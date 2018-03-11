using System.Collections.Generic;
using System.Linq;
using Autofac;
using Wrido.Execution;
using Wrido.Queries;
using Wrido.Resources;
using IQueryProvider = Wrido.Queries.IQueryProvider;

namespace Wrido.Plugin
{
  public class  PluginModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder
        .RegisterType<AssemblyPluginLoader>()
        .AsSelf()
        .SingleInstance();

      builder
        .RegisterType<NugetPluginLoader>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<PluginServiceProvider>()
        .AsSelf()
        .SingleInstance();

      builder
        .Register(c => c.ResolvePluginServices<IQueryProvider>())
        .InstancePerDependency();

      builder
        .Register(c => c.ResolvePluginServices<EmbeddedResource>())
        .InstancePerDependency();

      builder
        .Register(c => Enumerable.Concat(
          c.ResolvePluginServices<IResultExecuter>(),
          new IResultExecuter[]{ c.Resolve<WebResultExecuter>() }))
        .InstancePerDependency();
    }
  }
}
