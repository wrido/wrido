using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wrido.Configuration;
using Wrido.Electron;
using Wrido.Logging;
using Wrido.Plugin;
using Wrido.Plugin.Spotify;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddConfiguredSignalR();
      services.AddMvc();
      services.AddCreateReactAppFiles();
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
      builder
        .RegisterModule<QueryModule>()
        .RegisterModule<LoggingModule>()
        .RegisterModule<ConfigurationModule>()
        .RegisterModule<ResourceAspNetModule>()
        .RegisterModule<PluginModule>()
        .RegisterModule<ElectronModule>();

      builder
        .RegisterType<QueryService>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<ExecutionService>()
        .AsImplementedInterfaces()
        .SingleInstance();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app
        .UseDeveloperExceptionPage()
        .UseSignalR(hub =>
        {
          hub.MapHub<QueryHub>("/query");
          hub.MapHub<LoggingHub>("/logging");
        })
        .UseMvc()
        .UseCreateReactiveApp(useReactDevServer: env.IsDevelopment());
    }
  }
}
