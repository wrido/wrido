using Autofac;
using Autofac.Core;

namespace Wrido.Plugin
{
  public interface IWridoPlugin
  {
    void Register(ContainerBuilder builder);
  }
}
