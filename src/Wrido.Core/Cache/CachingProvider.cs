using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Configuration;
using Wrido.Logging;
using Wrido.Queries;
using Wrido.Queries.Events;

namespace Wrido.Cache
{
  public class CachingQueryProvider : IQueryProvider
  {
    private readonly IQueryProvider _actualProvider;
    private readonly TimeSpan _expires;
    private readonly ILogger _logger = LogManager.GetLogger<CachingQueryProvider>();
    private static ConcurrentDictionary<string, List<QueryEvent>> _eventCache = new ConcurrentDictionary<string, List<QueryEvent>>();

    public CachingQueryProvider(IQueryProvider actualProvider, IConfigurationProvider configProvider, TimeSpan expires, IEqualityComparer<string> comparer = default)
    {
      comparer = comparer ?? StringComparer.InvariantCultureIgnoreCase;
      _actualProvider = actualProvider;
      _expires = expires;
      configProvider.ConfigurationUpdated += (sender, args) => _eventCache.Clear();
    }

    public async Task QueryAsync(IQuery query, IObserver<QueryEvent> observer, CancellationToken ct)
    {
      if (_eventCache.TryGetValue(query.Argument, out var cachedStream))
      {
        _logger.Debug("Found cached query {rawQuery} with {queryEventCount} events", query.Raw, cachedStream.Count);
        foreach (var cachedEvent in cachedStream)
        {
          observer.OnNext(cachedEvent);
        }
      }
      else
      {
        _logger.Debug("Cache missed for {rawQuery}. Executing query and saving it to cache", query.Raw);
        var recorder = new RecordingObserver(observer);
        await _actualProvider.QueryAsync(query, recorder, ct);
        ct.ThrowIfCancellationRequested();
        _eventCache.TryAdd(query.Argument, recorder.Recorded);

        _logger.Verbose("Setting up cache expire handling in {expires}.", _expires);
        Timer timer = null;
        timer = new Timer(state =>
        {
          _logger.Information("Cache for {rawQuery} has expired. Evicting from cache", query.Raw);
          _eventCache.TryRemove(query.Argument, out _);
          timer?.Dispose();
        }, query.Raw, _expires, new TimeSpan(-1));
      }
    }

    public bool CanHandle(IQuery query) => _actualProvider.CanHandle(query);

    private class RecordingObserver : IObserver<QueryEvent>
    {
      public List<QueryEvent> Recorded { get; private set; }
      private readonly IObserver<QueryEvent> _underlying;

      public RecordingObserver(IObserver<QueryEvent> underlying)
      {
        _underlying = underlying;
        Recorded = new List<QueryEvent>();
      }

      public void OnCompleted() => _underlying.OnCompleted();
      public void OnError(Exception error) => _underlying.OnError(error);

      public void OnNext(QueryEvent value)
      {
        if (value is ResultAvailable available)
        {
          Recorded.Add(value);
        }
        _underlying.OnNext(value);
      }
    }
  }
}
