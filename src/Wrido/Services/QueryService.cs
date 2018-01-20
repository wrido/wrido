using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Wrido.Core;
using Wrido.Messages;
using IQueryProvider = Wrido.Core.IQueryProvider;

namespace Wrido.Services
{
  public interface IQueryService
  {
    Task QueryAsync(IClientProxy caller, string rawQuery);
  }

  public class QueryService : IQueryService
  {
    private readonly IEnumerable<IQueryProvider> _queryProviders;
    private CancellationTokenSource _currentQuery;

    public QueryService(IEnumerable<IQueryProvider> queryProviders)
    {
      _queryProviders = queryProviders;
    }

    public async Task QueryAsync(IClientProxy caller, string rawQuery)
    {
      // Cancel ongoing query
      var currentQuery = new CancellationTokenSource();
      var oldQuery = Interlocked.Exchange(ref _currentQuery, currentQuery);
      oldQuery?.Cancel();
      var ct = currentQuery.Token;

      // Notify about received query
      var query = new Query(rawQuery);
      await caller.InvokeAsync(new QueryReceived(query), ct);

      // Notify about number of providers
      var providers = GetProviders(query);
      await caller.InvokeAsync(QueryExecuting.By(providers), ct);

      // Execute query
      var allTasks = providers
        .Select(p => Task.Run(async () =>
        {
          var results = await p.QueryAsync(query, ct);
          await caller.InvokeAsync(new ResultsAvailable(query.Id, results), ct);
          return results;
        }, ct))
        .ToList();

      await Task.WhenAll(allTasks);
      var allResults = allTasks.Where(t => t.IsCompleted).SelectMany(t => t.Result).ToList();
      await caller.InvokeAsync(new QueryCompleted(query.Id, allResults), ct);
    }

    public IList<IQueryProvider> GetProviders(Query query)
    {
      return _queryProviders
        .Where(q => q.CanHandle(query))
        .ToList()
        .AsReadOnly();
    }
  }
}
