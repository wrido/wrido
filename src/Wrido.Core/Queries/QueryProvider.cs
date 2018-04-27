using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Logging;
using Wrido.Queries.Events;

namespace Wrido.Queries
{
  public interface IQueryProvider
  {
    Task QueryAsync(IQuery query, IObserver<QueryEvent> observer, CancellationToken ct);
    bool CanHandle(IQuery query);
  }

  public abstract class QueryProvider : IQueryProvider, IDisposable
  {
    private bool _isDisposed;
    private IObserver<QueryEvent> _observer;
    private readonly ILogger _logger = LogManager.GetLogger<QueryProvider>();

    protected void Available(IEnumerable<QueryResult> results) => Available(results.ToArray());
    protected void Available(params QueryResult[] results)
    {
      NotifyObservers(results.Select(r => new ResultAvailable(r)));
    }

    protected void Updated(IEnumerable<QueryResult> results) => Updated(results.ToArray());
    protected void Updated(params QueryResult[] results)
    {
      NotifyObservers(results.Select(r => new ResultUpdated(r)));
    }

    protected void Expired(IEnumerable<QueryResult> results) => Expired(results.ToArray());
    protected void Expired(params QueryResult[] results)
    {
      NotifyObservers(results.Select(r => new ResultExpired{ResultId = r.Id}));
    }

    private void NotifyObservers(IEnumerable<QueryEvent> events)
    {
      foreach (var @event in events)
      {
        _observer.OnNext(@event);
      }
    }

    protected IObserver<QueryEvent> Observer
    {
      get
      {
        if (!_isDisposed)
        {
          return _observer;
        }

        _logger.Warning("Accessing 'Observer' on a disposed QueryProvider is not supported.");
        throw new ObjectDisposedException(nameof(Observer));
      }

      set
      {
        if (_observer != null)
        {
          throw new NotSupportedException($"Observer allready assigned. Make sure that {GetType().Name} is registered with correct lifetime scope.");
        }
        if (_isDisposed)
        {
          throw new ObjectDisposedException(nameof(Observer));
        }
        _observer = value;
      }
    }

    public void Dispose()
    {
      _isDisposed = true;
    }

    public Task QueryAsync(IQuery query, IObserver<QueryEvent> observer, CancellationToken ct)
    {
      Observer = observer;
      return QueryAsync(query, ct);
    }

    public abstract bool CanHandle(IQuery query);

    protected abstract Task QueryAsync(IQuery query, CancellationToken ct);
  }
}