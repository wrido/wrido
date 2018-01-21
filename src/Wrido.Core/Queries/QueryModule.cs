using Autofac;

namespace Wrido.Queries
{
  public class QueryModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder
        .RegisterType<WebResultExecuter>()
        .AsImplementedInterfaces()
        .SingleInstance();
    }
  }
}
