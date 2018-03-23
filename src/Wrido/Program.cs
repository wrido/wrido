using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using ElectronNET.API;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Wrido.Electron;
using Wrido.Logging;

namespace Wrido
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var cts = new CancellationTokenSource();
      Console.CancelKeyPress += (sender, cancelArgs) => cts.Cancel();
      MainAsync(args, cts.Token).GetAwaiter().GetResult();
    }

    public static async Task MainAsync(string[] args, CancellationToken ct)
    {
      Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console(outputTemplate: LogTemplates.Console)
        .WriteTo.Elasticsearch()
        .CreateLogger();
      LogManager.LoggerFactory = type => new SerilogLogger(Log.ForContext(type));

      Log.Information("Application started with {applicationArgs}", args);

      try
      {
        var webHost = WebHost.CreateDefaultBuilder(args)
          .ConfigureServices(services => services.AddAutofac())
          .UseSerilog()
          .UseElectron(args)
          .UseStartup<Startup>()
          .Build();

        await webHost.StartAsync(ct);
        
        var electronHost = webHost.Services.GetService<IElectronHost>();
        await electronHost.StartAsync(ct);

        var appLifeTime = webHost.Services.GetService<IApplicationLifetime>();
        var tsc = new TaskCompletionSource<int>();
        appLifeTime.ApplicationStopped.Register(() => tsc.TrySetResult(1));
        ct.Register(() => appLifeTime.StopApplication());
        await tsc.Task;
      }
      catch (Exception e)
      {
        Log.Fatal(e, "Host terminated unexpectedly");
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }
  }
}
