using System;
using Autofac;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Sockets;
using Wrido.Configuration;
using Wrido.Plugin.Spotify.Common;

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
          spotifyCfg.GivePermissionUri = new Uri($"{appCfg.ServerUrl}spotify/auth");
          spotifyCfg.AccessTokenUri = new Uri($"{appCfg.ServerUrl}spotify/auth/result");
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
        .RegisterType<SpotifyExecutor>()
        .AsImplementedInterfaces()
        .SingleInstance();
    }
  }
}
