using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Wrido.Core;
using Wrido.Core.Logging;
using Wrido.Logging;
using Wrido.Messages;
using IQueryProvider = Wrido.Core.IQueryProvider;

namespace Wrido.Query
{
  public interface IQueryService
  {
    Task QueryAsync(IClientProxy caller, string rawQuery);
  }

  public class QueryService : IQueryService
  {
    private readonly IEnumerable<IQueryProvider> _queryProviders;
    private readonly ILogger _logger;
    private CancellationTokenSource _currentQuery;

    public QueryService(IEnumerable<IQueryProvider> queryProviders, ILogger logger)
    {
      _queryProviders = queryProviders;
      _logger = logger;
    }

    public async Task QueryAsync(IClientProxy caller, string rawQuery)
    {
      CancelOngoingQuery(out var currentCt);

      var query = new Core.Query(rawQuery);
      using (_logger.BeginScope(LogProperties.QueryId, query.Id))
      {
        currentCt.ThrowIfCancellationRequested();
        _logger.Verbose("Notifying frontend that query is received.");
        await caller.InvokeAsync(new QueryReceived(query), currentCt);

        var providers = GetProviders(query);
        _logger.Verbose("Notifying frontend that query will be handled by {providerCount} providers.", providers.Count);
        await caller.InvokeAsync(QueryExecuting.By(providers), currentCt);

        // Execute query
        var allTasks = providers.Select(p => Task.Run(async () =>
          {
            var providerName = p.GetType().Name;
            using (var operation = _logger.Timed("Query from {queryProvider}", providerName))
            {
              try
              {
                var results = (await p.QueryAsync(query, currentCt)).ToList();
                currentCt.ThrowIfCancellationRequested();
                _logger.Debug("Provider {queryProvider} executed query successfully. {resultCount} results available.", providerName, results.Count);
                await caller.InvokeAsync(new ResultsAvailable(query.Id, results), currentCt);
                return results;
              }
              catch (Exception e)
              {
                operation.Cancel();
                throw;
              }
            }
          }, currentCt))
          .ToList();

        // Wait for provider tasks
        await Task.WhenAll(allTasks).ContinueWith(async _ =>
        {
          Task.WaitAll(allTasks.ToArray<Task>());
          var allResults = allTasks.Where(t => t.IsCompleted).SelectMany(t => t.Result).ToList();

          if (allTasks.Any(t => !t.IsCompletedSuccessfully))
          {
            _logger.Debug("One or more sub queries have failed.");
            await caller.InvokeAsync(new QueryCancelled(query.Id), currentCt);
          }
          else
          {
            _logger.Verbose("Query completed. Total result count is {resultCount}", allResults.Count);
            await caller.InvokeAsync(new QueryCompleted(query.Id, allResults), currentCt);
          }
        });
      }
    }

    private IList<IQueryProvider> GetProviders(Core.Query query)
    {
      return _queryProviders.Where(q => q.CanHandle(query))
        .ToList()
        .AsReadOnly();
    }

    private void CancelOngoingQuery(out CancellationToken nextToken)
    {
      var currentQuery = new CancellationTokenSource();
      nextToken = currentQuery.Token;
      var oldQuery = Interlocked.Exchange(ref _currentQuery, currentQuery);
      using (TimedOperationExtensions.Timed(_logger, LogLevel.Debug, "Cancel ongoing query"))
      {
        oldQuery?.Cancel();
      }
    }
  }
}
