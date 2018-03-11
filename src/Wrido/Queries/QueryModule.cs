using Autofac;
using Wrido.Execution;

namespace Wrido.Queries
{
  public class QueryModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder
        .RegisterType<ClientStreamRepository>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<WebResultExecuter>()
        .AsSelf()
        .SingleInstance();
    }
  }
}
