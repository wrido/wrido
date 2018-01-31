using System.Net.Http;
using Autofac;
using Newtonsoft.Json;
using Wrido.Resources;

namespace Wrido.Plugin.Wikipedia
{
  public class WikipediaPlugin : IWridoPlugin
  {
    public void Register(ContainerBuilder builder)
    {
      builder
        .Register(c => new HttpClient())
        .SingleInstance();

      builder
        .RegisterType<WikipediaProvider>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<WikipediaResponseConverter>()
        .AsSelf();

      builder
        .Register(c => new JsonSerializer
        {
          Converters =
          {
            c.Resolve<WikipediaResponseConverter>()
          }
        });
    }
  }
}
