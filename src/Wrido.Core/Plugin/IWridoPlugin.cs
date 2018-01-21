using Autofac;

namespace Wrido.Plugin
{
  public interface IWridoPlugin
  {
    void Register(ContainerBuilder builder);
  }
}
