using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Everything;
using Everything.LowLevel.SyntacticSugar;
using Everything.Model;
using Everything.Search;
using Wrido.Logging;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido.Plugin.Everything
{
  public class EverythingProvider : QueryProvider
  {
    private readonly IEverythingClient _everything;
    private readonly ICategoryProvider _category;
    private readonly EverythingConfiguration _config;
    private readonly ILogger _logger;
    private readonly SearchOptions _searchOption;

    public EverythingProvider(IEverythingClient everything, ICategoryProvider category, EverythingConfiguration config, ILogger logger)
    {
      _everything = everything;
      _category = category;
      _config = config;
      _logger = logger;
      _searchOption = new SearchOptions
      {
        RequestFlags = RequestFlags.FileName | RequestFlags.Path | RequestFlags.DateCreated | RequestFlags.DateAccessed | RequestFlags.DateModified | RequestFlags.Size,
        MaxResult = 20,
        Sort = Sort.RunCountDescending
      };
    }

    public override bool CanHandle(IQuery query)
    {
      return string.Equals(query?.Command, _config?.Keyword, StringComparison.CurrentCultureIgnoreCase);
    }

    protected override async Task QueryAsync(IQuery query, CancellationToken ct)
    {
      if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
      {
        Available(new EverythingNotAvailableResult("Everything is only available on Windows"));
        return;
      }

      var everythingStatus = _everything.GetStatus();
      if (!everythingStatus.IsRunning)
      {
        var notRunning = new WebResult
        {
          Icon = EverythingIcon.EverythingLogo,
          Title = "Everything not running",
          Description = "Not installed? Execute this result to go to download page",
          Uri = new Uri("https://www.voidtools.com/downloads/")
        };

        Available(notRunning);

        while (!_everything.GetStatus().IsRunning)
        {
          await Task.Delay(TimeSpan.FromSeconds(1), ct);
        }

        Expired(notRunning);
        if(!_everything.GetStatus().IsDatabaseLoaded)
        {
          var dbLoading = new EverythingNotAvailableResult("Everything is starting up")
          {
            Icon = EverythingIcon.EverythingLogo,
            Description = "This may take a few seconds",
          };

          Available(dbLoading);
          while (!_everything.GetStatus().IsDatabaseLoaded)
          {
            await Task.Delay(TimeSpan.FromSeconds(1), ct);
          }
          Expired(dbLoading);
        }
      }

      SearchResult result;
      using (_logger.Timed("Search everything for {rawQuery}", query.Argument))
      {
        result = await _everything.SearchAsync(query.Argument, _searchOption, ct);
      }

      ct.ThrowIfCancellationRequested();


      foreach (var item in result.Items)
      {
        Available(new EverythingResult
        {
          Title = item.Name,
          Description = item.FullPath,
          Category = _category.Get(item),
          FullPath = item.FullPath,
          Icon = new Image
          {
            Alt = item.Name,
            Uri = (item is FileResultItem)
              ? new Uri($"/icons/{item.FullPath}", UriKind.Relative)
              : EverythingIcon.FolderIcon.Uri
          }
        });
      }
    }
  }
}
