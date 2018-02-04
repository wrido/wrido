using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Logging;
using Wrido.Queries.Events;

namespace Wrido.Queries
{
  public interface IQueryService
  {
    IObservable<QueryEvent> StreamQueryEvents(string rawQuery);
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

    public IObservable<QueryEvent> StreamQueryEvents(string rawQuery) =>
      Observable.Create<QueryEvent>(async (observer, externalCt) =>
      {
        var query = new Query(rawQuery);
        using (_logger.BeginScope(LogProperties.QueryId, query.Id))
        using (_logger.Timed("Query {rawQuery}", rawQuery))
        {
          CancelOngoingQuery(externalCt, out var currentCt);
          currentCt.Register(() => observer.OnNext(new QueryCancelled(query.Id)));

          _logger.Verbose("Notifying observers that query is received.");
          observer.OnNext(new QueryReceived(query));
          var providers = GetProviders(query);
          _logger.Verbose("Notifying observers that query will be handled by {providerCount} providers.", providers.Count);
          observer.OnNext(QueryExecuting.By(providers));

          var providerTasks = providers
            .Select(async provider =>
            {
              var operation = _logger.Timed("{queryProvider} provider", provider.GetType().Name);
              var providerObserver = Observer.Create<QueryEvent>(observer.OnNext, operation.Cancel, operation.Complete);
              try
              {
                await provider.QueryAsync(query, providerObserver, currentCt);
              }
              catch (OperationCanceledException) { /* Cancellation is OK */ }
              catch (Exception e)
              {
                _logger.Information(e, "An unhandle exception was thrown.");
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
      });

    private IList<IQueryProvider> GetProviders(Query query)
    {
      return _queryProviders.Where(q => q.CanHandle(query))
        .ToList()
        .AsReadOnly();
    }

    private void CancelOngoingQuery(CancellationToken streamToken, out CancellationToken nextToken)
    {
      nextToken = new CancellationToken();
      var currentQuery = CancellationTokenSource.CreateLinkedTokenSource(streamToken, nextToken);
      nextToken = currentQuery.Token;

      var oldQuery = Interlocked.Exchange(ref _currentQuery, currentQuery);
      using (_logger.Timed(LogLevel.Debug, "Cancel ongoing query"))
      {
        oldQuery?.Cancel();
      }
    }
  }
}
