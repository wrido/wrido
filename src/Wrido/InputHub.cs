using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Wrido.Core;
using Wrido.Messages;
using Wrido.Services;

namespace Wrido
{
  public class InputHub : Hub
  {
    private readonly IQueryService _queryService;

    public InputHub(IQueryService queryService)
    {
      _queryService = queryService;
    }

    public async Task QueryAsync(string rawQuery)
    {
      var caller = Clients.Client(Context.ConnectionId);
      await _queryService.QueryAsync(caller, rawQuery);
    }
  }
}
