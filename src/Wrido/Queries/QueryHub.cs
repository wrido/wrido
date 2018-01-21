using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Wrido.Logging;
using ILogger = Wrido.Logging.ILogger;

namespace Wrido.Queries
{
  public class QueryHub : Hub
  {
    private readonly IQueryService _queryService;
    private readonly ILogger _logger = new SerilogLogger(Log.ForContext<QueryHub>());

    public QueryHub(IQueryService queryService)
    {
      _queryService = queryService;
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
  }
}
