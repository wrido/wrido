using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Wrido.Core.Logging;
using Wrido.Logging;
using Wrido.Services;
using ILogger = Wrido.Core.Logging.ILogger;

namespace Wrido
{
  public class InputHub : Hub
  {
    private readonly IQueryService _queryService;
    private readonly ILogger _logger = new SerilogLogger(Log.ForContext<InputHub>());

    public InputHub(IQueryService queryService)
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
