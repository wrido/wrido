using System.Collections.Generic;
using Autofac;
using Autofac.Core;

namespace Wrido.Plugin
{
  public static class ComponentContextPluginExtension
  {
    public static IEnumerable<TService> ResolvePluginServices<TService>(this IComponentContext context)
    {
      if (!context.IsRegistered<PluginServiceProvider>())
      {
        throw new DependencyResolutionException("Unable to resolve plugins without PluginServiceProvider registered");
      }
      return context.Resolve<PluginServiceProvider>().GetServices<TService>();
    }
  }
}
