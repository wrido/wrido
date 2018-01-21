using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Wrido.Core.Queries;
using Wrido.Logging;
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

    public async Task QueryAsync(string rawQuery)
    {
      using (_logger.Timed("Query {rawQuery}", rawQuery))
      {
        try
        {
          var caller = Clients.Client(Context.ConnectionId);
          await _queryService.QueryAsync(caller, rawQuery);
        }
        catch (TaskCanceledException){ /* do nothing */ }
        catch (Exception e)
        {
          _logger.Information(e, "An unhandled exception occured.");
        }
      }
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
