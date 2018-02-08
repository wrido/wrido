using System;
using System.Collections.Generic;
using Autofac;
using Wrido.Queries;

namespace Wrido.Cache
{
  public static class AutofacCachingExtension
  {
    public static ContainerBuilder AddCaching<TProvider>(
      this ContainerBuilder builder,
      TimeSpan expires,
      IEqualityComparer<string> comparer = default)
        where TProvider : IQueryProvider
    {
      builder
        .RegisterType<TProvider>()
        .AsSelf()
        .InstancePerDependency();

      builder
        .Register(c => new CachingQueryProvider(c.Resolve<TProvider>(), expires, comparer))
        .AsImplementedInterfaces()
        .InstancePerDependency();
      return builder;
    }
  }
}
