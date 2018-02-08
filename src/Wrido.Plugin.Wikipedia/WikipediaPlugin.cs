using System;
using System.Collections.Generic;
using System.Net.Http;
using Autofac;
using Newtonsoft.Json;
using Wrido.Cache;
using Wrido.Configuration;
using Wrido.Plugin.Wikipedia.Common;
using Wrido.Plugin.Wikipedia.Serialization;

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
        .AddCaching<WikipediaProvider>(TimeSpan.FromHours(2));

      builder
        .RegisterType<WikipediaSearchConverter>()
        .AsSelf();

      builder
        .Register(c => new JsonSerializer
        {
          Converters = { c.Resolve<WikipediaSearchConverter>() }
        });

      builder
        .Register(c => c
          .Resolve<IConfigurationProvider>()
          .GetConfiguration<WikipediaConfiguration>() ?? WikipediaConfiguration.Fallback)
        .AsSelf()
        .SingleInstance();

      builder
        .Register(c =>
        {
          var pluginConfig = c.Resolve<WikipediaConfiguration>();
          var clients = new List<IWikipediaClient>();
          foreach (var baseUrl in pluginConfig.BaseUrls)
          {
            var httpClient = new HttpClient
            {
              BaseAddress = new Uri(baseUrl)
            };
            var client = new WikipediaClient(httpClient, c.Resolve<JsonSerializer>());
            clients.Add(client);
          }
          return clients;
        }).As<IEnumerable<IWikipediaClient>>().SingleInstance();
    }
  }
}
