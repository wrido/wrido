using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Wrido
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var cts = new CancellationTokenSource();
      Console.CancelKeyPress += (sender, cancelArgs) => cts.Cancel();

      MainAsync(args, cts.Token)
        .GetAwaiter()
        .GetResult();
    }

    public static async Task MainAsync(string[] args, CancellationToken ct)
    {
      var host = BuildWebHost(args);
      await host.RunAsync(ct);
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .Build();
  }
}
