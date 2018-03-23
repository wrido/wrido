using System;
using System.Threading;
using System.Threading.Tasks;
using Everything;
using Everything.LowLevel.SyntacticSugar;
using Everything.Model;
using Everything.Search;
using Wrido.Logging;
using Wrido.Queries;

namespace Wrido.Plugin.Everything
{
  public class EverythingProvider : QueryProvider
  {
    private readonly IEverythingClient _everything;
    private readonly ILogger _logger;
    private readonly SearchOptions _searchOption;

    public EverythingProvider(IEverythingClient everything, ILogger logger)
    {
      _everything = everything;
      _logger = logger;
      _searchOption = new SearchOptions
      {
        RequestFlags = RequestFlags.FileName | RequestFlags.Path | RequestFlags.DateCreated | RequestFlags.DateAccessed | RequestFlags.DateModified | RequestFlags.Size,
        MaxResult = 20
      };
    }

    public override bool CanHandle(Query query)
    {
      return !query?.Command?.StartsWith(":") ?? false;
    }

    protected override async Task QueryAsync(Query query, CancellationToken ct)
    {
      _logger.Information("Handling query {rawQuery}", query.Raw);

      SearchResult result;
      using (_logger.Timed("Search everything for {rawQuery}", query.Raw))
      {
        result = await _everything.SearchAsync(query.Raw, _searchOption, ct);
      }

      await Task.Delay(1000);
      _logger.Information("Obvious delay completed");

      foreach (var item in result.Items)
      {
        if (ct.IsCancellationRequested)
        {
          _logger.Information("Query {rawQuery} is cancelled", query.Raw);
          return;
        }

        _logger.Information("Preparing result for {itemName}", item.Name);
        Available(new EverythingResult
        {
          Title = item.Name,
          Description = item.FullPath
        });
      }
    }
  }
}
