using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Wrido.Configuration;
using Wrido.Electron;
using Wrido.Logging;
using Wrido.Plugin.Dummy;
using Wrido.Plugin.Google;
using Wrido.Plugin.StackExchange;
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
      services.AddSignalR(options => options.JsonSerializerSettings = new JsonSerializerSettings
      {
        NullValueHandling = NullValueHandling.Ignore,
        TypeNameHandling = TypeNameHandling.Auto,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
      });
      services.AddMvc();
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
      builder
        .RegisterModule<QueryModule>()
        .RegisterModule<LoggingModule>()
        .RegisterModule<ConfigurationModule>()
        .RegisterModule<ResourceAspNetModule>()
        .RegisterModule<ElectronModule>();

      builder
        .RegisterType<QueryService>()
        .AsImplementedInterfaces()
        .SingleInstance();

      builder
        .RegisterType<ExecutionService>()
        .AsImplementedInterfaces()
        .SingleInstance();

      new GooglePlugin().Register(builder);
      new StackExchangePlugin().Register(builder);
      new DummyPlugin().Register(builder);
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app
        .UseDeveloperExceptionPage()
        .UseDefaultFiles()
        .UseStaticFiles()
        .UseSignalR(hub =>
          {
            hub.MapHub<QueryHub>("query");
            hub.MapHub<LoggingHub>("logging");
          })
        .UseMvcWithDefaultRoute();
    }
  }
}
