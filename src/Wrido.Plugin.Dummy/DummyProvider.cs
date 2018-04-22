using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido.Plugin.Dummy
{
  public class DummyProvider : QueryProvider
  {
    private readonly TimeSpan _minDuration;
    private readonly TimeSpan _maxDuratin;
    private readonly string _name;
    private readonly Random _random;
    private readonly Image _iconResource;
    private readonly List<string> _categories = new List<string>{ "Application", "Executable", "Compressed Archieve", "Music" };

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

    public override bool CanHandle(Query query)
    {
      if (string.IsNullOrEmpty(query?.Raw))
      {
        return false;
      }
      return !query?.Command?.StartsWith(":") ?? true;
    }

    protected override async Task QueryAsync(Query query, CancellationToken ct)
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
          PreviewUri = new Uri("/resources/wrido/plugin/dummy/resources/preview.htm", UriKind.Relative),
          Category = _categories[_random.Next(0, _categories.Count -1)]
        };
        Available(result);

        if (_random.NextDouble() > 0.5)
        {
          await Task.Delay(duration, ct);
          result.Title = $"{result.Title} - UPDATED!";

          Updated(result);
        }
      }
    }
  }
}
