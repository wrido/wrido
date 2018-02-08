using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Wrido.Logging;
using Wrido.Plugin.Wikipedia;
using Wrido.Queries.Events;
using ILogger = Wrido.Logging.ILogger;

namespace Wrido.Queries
{
  public class QueryHub : Hub
  {
    private readonly IQueryService _queryService;
    private readonly IExecutionService _executionService;
    private readonly IClientStreamRepository _streamRepo;
    private readonly ILogger _logger = new SerilogLogger(Log.ForContext<QueryHub>());

    public QueryHub(IQueryService queryService, IExecutionService executionService, IClientStreamRepository streamRepo)
    {
      _queryService = queryService;
      _executionService = executionService;
      _streamRepo = streamRepo;
    }

    public IObservable<QueryEvent> CreateResponseStream() =>_streamRepo.GetOrAddObservable(Context.ConnectionId);

    public async Task QueryAsync(string rawQuery)
    {
      if (!_streamRepo.TryGetObserver(Context.ConnectionId, out var observer))
      {
        throw new Exception();
      }

      await _queryService.QueryAsync(rawQuery, observer);
    }

    public async Task EventBasedQueryAsync(string rawQuery)
    {
      var client = Clients.Client(Context.ConnectionId);
      var observer = Observer.Create<QueryEvent>(@event =>
      {
        client.InvokeAsync(@event).GetAwaiter().GetResult();
      });
      await _queryService.QueryAsync(rawQuery, observer);
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
