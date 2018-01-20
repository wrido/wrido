using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Wrido
{
  public class InputHub : Hub
  {
    private readonly Random _random;

    public InputHub()
    {
      _random = new Random();
    }

    public async Task QueryAsync(string rawQuery)
    {
      var numberOfResults = _random.Next(1, 10);
      var results = new List<string>();
      for (var i = 0; i < numberOfResults; i++)
      {
        results.Add($"{rawQuery} (result {i})");
      }

      await Clients.Client(Context.ConnectionId).InvokeAsync("ResultAvailable", results);
    }
  }
}
