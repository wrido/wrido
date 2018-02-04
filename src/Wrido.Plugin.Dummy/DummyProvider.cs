using System;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Queries;
using Wrido.Queries.Events;
using Wrido.Resources;

namespace Wrido.Plugin.Dummy
{
  public class DummyProvider : IQueryProvider
  {
    private readonly TimeSpan _minDuration;
    private readonly TimeSpan _maxDuratin;
    private readonly string _name;
    private readonly Random _random;
    private readonly Image _iconResource;

    public DummyProvider(TimeSpan minDuration, TimeSpan maxDuratin, string name, Uri iconUri)
    {
      _minDuration = minDuration;
      _maxDuratin = maxDuratin;
      _name = name;
      _random = new Random();
      _iconResource = new Image
      {
        Alt = name,
        Uri = iconUri
      };
    }

    public bool CanHandle(Query query)
    {
      return true;
    }

    public async Task QueryAsync(Query query, IObserver<QueryEvent> observer, CancellationToken ct)
    {
      var numberOfResults = _random.Next(1, 9);
      for (var i = 0; i < numberOfResults; i++)
      {
        var duration = new TimeSpan(_random.Next((int)_minDuration.Ticks / numberOfResults, (int)_maxDuratin.Ticks / numberOfResults));
        await Task.Delay(duration, ct);
        var result = new QueryResult
        {
          Title = $"[{_name}][{i}]: {query.Raw}",
          Description = $"Delayed with {duration.TotalMilliseconds} ms.",
          Icon = _iconResource,
          Renderer = new Script("/resources/wrido/plugin/dummy/resources/render.js")
        };
        observer.OnNext(new ResultAvailable(result));

        if (_random.NextDouble() > 0.5)
        {
          await Task.Delay(duration, ct);
          result.Title = $"{result.Title} - UPDATED!";

          observer.OnNext(new ResultUpdated(result));
        }
      }
    }
  }
}
