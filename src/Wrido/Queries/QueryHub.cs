using System;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Wrido.Logging;
using Wrido.Queries.Events;
using ILogger = Wrido.Logging.ILogger;

namespace Wrido.Queries
{
  public class QueryHub : Hub
  {
    private readonly IQueryService _queryService;
    private readonly IExecutionService _executionService;
    private readonly ILogger _logger = new SerilogLogger(Log.ForContext<QueryHub>());

    public QueryHub(IQueryService queryService, IExecutionService executionService)
    {
      _queryService = queryService;
      _executionService = executionService;
    }

    public IObservable<QueryEvent> StreamQueryEvents(string rawQuery) => _queryService.StreamQueryEvents(rawQuery);

    public async Task QueryAsync(string rawQuery)
    {
      var caller = Clients.Client(Context.ConnectionId);
      var completeTsc = new TaskCompletionSource<bool>();
      var observer = Observer.Create<QueryEvent>(
        @event => caller.InvokeAsync(@event),
        () => completeTsc.TrySetResult(true)
      );

      var resultStream = _queryService.StreamQueryEvents(rawQuery);
      var subscription = resultStream.Subscribe(observer);
      await completeTsc.Task;
      subscription.Dispose();
    }

    public async Task ExecuteAsync(QueryResult result)
    {
      using (_logger.Timed("Execute {@queryResult}", result))
      {
        try
        {
          var caller = Clients.Client(Context.ConnectionId);
          await _executionService.ExecuteAsync(caller, result);
        }
        catch (Exception e)
        {
          _logger.Information(e, "An unhandled exception occured.");
        }
      }
    }
  }
}
