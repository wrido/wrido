using Autofac;

namespace Wrido.Core.Plugin
{
  public interface IWridoPlugin
  {
    void Register(ContainerBuilder builder);
  }
}
