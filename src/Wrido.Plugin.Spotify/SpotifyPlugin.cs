using System;
using System.Net.Http;
using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wrido.Configuration;
using Wrido.Plugin.Spotify.Authorization;
using Wrido.Plugin.Spotify.Common;
using Wrido.Plugin.Spotify.Common.Authorization;
using Wrido.Plugin.Spotify.Common.Search;
using Wrido.Plugin.Spotify.Playback;

namespace Wrido.Plugin.Spotify
{
  public class SpotifyPlugin : IWridoPlugin
  {
    public void Register(ContainerBuilder builder)
    {
      builder
        .Register(c =>
        {
          var cfgProvider = c.Resolve<IConfigurationProvider>();
          var appCfg = cfgProvider.GetAppConfiguration();

          var spotifyCfg = cfgProvider.GetConfiguration<SpotifyConfiguration>() ?? SpotifyConfiguration.Default;
          spotifyCfg.Keyword = spotifyCfg.Keyword ?? ":s";
          spotifyCfg.RefreshAccessUri = new Uri($"{appCfg.ServerUrl}spotify/refresh");
          return spotifyCfg;
        })
        .InstancePerDependency();

      builder
        .RegisterType<SpotifyProvider>()
        .AsImplementedInterfaces()
        .InstancePerDependency();

      builder
        .RegisterType<SpotifyClient>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<WridoAccessTokenProvider>()
        .AsSelf()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<QueryParameterBuilder>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<HttpClient>()
        .AsSelf()
        .SingleInstance();

      builder
        .Register(c => new JsonSerializer
        {
          NullValueHandling = NullValueHandling.Ignore,
          ContractResolver = new CamelCasePropertyNamesContractResolver
          {
            NamingStrategy = new SnakeCaseNamingStrategy()
          }
        })
        .AsSelf()
        .SingleInstance();

      builder
        .RegisterType<AuthorizationExecutor>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<ChangeSongExecuter>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<PlayPauseExecutor>()
        .AsImplementedInterfaces()
        .SingleInstance();
    }
  }
}
