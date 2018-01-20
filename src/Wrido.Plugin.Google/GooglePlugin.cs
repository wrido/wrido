using System;
using System.Net.Http;
using Autofac;
using Autofac.Core;
using Wrido.Core.Plugin;

namespace Wrido.Plugin.Google
{
  public class GooglePlugin : IWridoPlugin
  {
    public void Register(ContainerBuilder builder)
    {
      builder
        .Register(GoogleHttpClient)
        .Named<HttpClient>(nameof(GoogleHttpClient));

      builder
        .RegisterType<GoogleProvider>()
        .AsImplementedInterfaces()
        .WithParameter(new ResolvedParameter(
          (info, context) => info.ParameterType == typeof(HttpClient),
          (info, context) => context.ResolveNamed<HttpClient>(nameof(GoogleHttpClient))))
        .SingleInstance();
    }

    private static HttpClient GoogleHttpClient(IComponentContext context)
    {
      return new HttpClient
      {
        BaseAddress = new Uri("https://www.google.com/complete/search")
      };
    }
  }
}
