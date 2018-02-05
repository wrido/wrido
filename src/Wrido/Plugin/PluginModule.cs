using Autofac;
using Wrido.Queries;
using Wrido.Resources;

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
    }
  }
}
