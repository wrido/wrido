using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Wrido.Configuration;
using Wrido.Configuration.Events;
using Wrido.Electron;
using Wrido.Electron.Windows;
using Wrido.Logging;
using Wrido.Queries.Events;
using ILogger = Wrido.Logging.ILogger;

namespace Wrido.Queries
{
  public class QueryHub : Hub
  {
    private readonly IQueryService _queryService;
    private readonly IExecutionService _executionService;
    private readonly IWindowsServices _windowServices;
    private readonly IConfigurationProvider _configProvider;
    private readonly IClientStreamRepository _streamRepo;
    private readonly ILogger _logger = new SerilogLogger(Log.ForContext<QueryHub>());

    public QueryHub(
      IQueryService queryService,
      IExecutionService executionService,
      IWindowsServices windowServices,
      IConfigurationProvider configProvider,
      IClientStreamRepository streamRepo)
    {
      _queryService = queryService;
      _executionService = executionService;
      _windowServices = windowServices;
      _configProvider = configProvider;
      _streamRepo = streamRepo;
    }

    public IObservable<BackendEvent> CreateResponseStream()
    {
      var observable = _streamRepo.GetOrAdd(Context.ConnectionId);
      _streamRepo.TryGetObserver(Context.ConnectionId, out var observer);
      _configProvider.ConfigurationUpdated += (sender, args) =>
      {
        observer.OnNext(new ConfigurationUpdated
        {
          Configuration = _configProvider.GetAppConfiguration()
        });
      };
      return observable;
    }

    public async Task QueryAsync(string rawQuery)
    {
      if (string.IsNullOrEmpty(rawQuery))
      {
        await _windowServices.ResizeShellAsync(ShellSize.InputFieldOnly);
      }
      else
      {
        await _windowServices.ResizeShellAsync(ShellSize.InputAndResults);
      }

      if (!_streamRepo.TryGetObserver(Context.ConnectionId, out var observer))
      {
        var client = Clients.Client(Context.ConnectionId);
        observer = Observer.Create<BackendEvent>(e => client.SendAsync(e).GetAwaiter().GetResult());
      }

      await _queryService.QueryAsync(rawQuery, observer);
    }

    public Task HideShellAsync() => _windowServices.HideShellAsync();

    public async Task ExecuteAsync(QueryResult result)
    {
      using (_logger.Timed("Execute {@queryResult}", result))
      {
        try
        {
          var caller = Clients.Client(Context.ConnectionId);
          await _executionService.ExecuteAsync(caller, result);
          await _windowServices.HideShellAsync(CancellationToken.None);
        }
        catch (Exception e)
        {
          _logger.Information(e, "An unhandled exception occured.");
        }
      }
    }

    public override Task OnConnectedAsync()
    {
      return Clients.Caller.SendAsync(new ConfigurationAvailable
      {
        Configuration = _configProvider.GetAppConfiguration()
      });
    }
  }
}
