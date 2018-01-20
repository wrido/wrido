using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
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
        .CreateLogger();

      try
      {
        var host = WebHost.CreateDefaultBuilder(args)
          .ConfigureServices(services => services.AddAutofac())
          .UseSerilog()
          .UseStartup<Startup>()
          .Build();

        await host.RunAsync(ct);
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
