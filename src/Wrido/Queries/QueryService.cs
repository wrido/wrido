using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Logging;
using Wrido.Queries.Events;

namespace Wrido.Queries
{
  public interface IQueryService
  {
    Task QueryAsync(string rawQuery, IObserver<QueryEvent> observer);
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

    public async Task QueryAsync(string rawQuery, IObserver<QueryEvent> observer)
    {
      var query = new Query(rawQuery);
      using (_logger.BeginScope(LogProperties.QueryId, query.Id))
      using (var fullQuery = _logger.Timed("Query {rawQuery}", rawQuery))
      {
        CancelOngoingQuery(out var currentCt);

        _logger.Verbose("Notifying observers that query is received.");
        observer.OnNext(new QueryReceived(query));
        var providers = GetProviders(query);
        _logger.Verbose("Notifying observers that query will be handled by {providerCount} providers.", providers.Count);
        observer.OnNext(QueryExecuting.By(providers));

        var providerTasks = providers
          .Select(async provider =>
          {
            var providerQuery = _logger.Timed("{queryProvider} provider", provider.GetType().Name);
            var providerObserver = Observer.Create<QueryEvent>(observer.OnNext, providerQuery.Cancel, providerQuery.Complete);
            try
            {
              currentCt.ThrowIfCancellationRequested();
              await provider.QueryAsync(query, providerObserver, currentCt);
            }
            catch (OperationCanceledException)
            {
              _logger.Information("Query {rawQuery} has been cancelled for {queryProvider}", rawQuery, provider.GetType().Name);
              fullQuery.Cancel();
              providerQuery.Cancel();
               /* Cancellation is OK */
            }
            catch (Exception e)
            {
              _logger.Information(e, "An unhandle exception was thrown.");
              providerQuery.Cancel(e);
            }
            finally
            {
              providerObserver.OnCompleted();
            }
          })
          .ToArray();
        await Task.WhenAll(providerTasks);
        observer.OnNext(new QueryCompleted(query.Id, null));
      }
    }

    private IList<IQueryProvider> GetProviders(Query query)
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
      using (_logger.Timed(LogLevel.Debug, "Cancel ongoing query"))
      {
        oldQuery?.Cancel();
      }
    }
  }
}
