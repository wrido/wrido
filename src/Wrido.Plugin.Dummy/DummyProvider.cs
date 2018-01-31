using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Queries;
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

    public async Task<IEnumerable<QueryResult>> QueryAsync(Query query, CancellationToken ct)
    {
      var numberOfResults = _random.Next(1, 9);
      var duration = new TimeSpan(_random.Next((int)_minDuration.Ticks, (int)_maxDuratin.Ticks));

      var result = new List<QueryResult>();
      for (var i = 0; i < numberOfResults; i++)
      {
        result.Add(new QueryResult
        {
          Title = $"[{_name}][{i}]: {query.Raw}",
          Description = $"Delayed with {duration.TotalMilliseconds} ms.",
          Icon = _iconResource,
          Renderer = new Script("/resources/wrido/plugin/dummy/resources/render.js")
        });
      }

      await Task.Delay(duration, ct);

      return result;
    }
  }
}
