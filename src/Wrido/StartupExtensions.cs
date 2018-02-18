using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;

namespace Wrido
{
  public static class StartupExtensions
  {
    private const string CreateReactBuildPath = "ClientApp/build";
    private const string CreateReactSourcePath = "ClientApp";
    private const string CreateReactStartScript = "start";

    public static void AddCreateReactAppFiles(this IServiceCollection collection)
    {
      collection.AddSpaStaticFiles(o => o.RootPath = CreateReactBuildPath);
    }

    public static IApplicationBuilder UseCreateReactiveApp(this IApplicationBuilder app, bool useReactDevServer = false)
    {
      app.UseSpaStaticFiles();
      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = CreateReactSourcePath;
        if (useReactDevServer)
        {
          spa.UseReactDevelopmentServer(CreateReactStartScript);
        }
      });
      return app;
    }
  }
}
