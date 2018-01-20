using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
      services.AddSignalR();
      services.AddMvc();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      app
        .UseDeveloperExceptionPage()
        .UseDefaultFiles()
        .UseStaticFiles()
        .UseSignalR(hub => hub.MapHub<InputHub>("input"))
        .UseMvc();
    }
  }
}
