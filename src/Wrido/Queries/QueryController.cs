using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Wrido.Queries
{
  public class QueryController : Controller
  {
    private readonly IQueryService _queryService;

    public QueryController(IQueryService queryService)
    {
      _queryService = queryService;
    }

    [HttpGet("api/query/{*rawQuery}")]
    public async Task<IActionResult> QueryAsync(string rawQuery)
    {
      var eventList = await _queryService
        .StreamQueryEvents(rawQuery)
        .ToList()
        .FirstOrDefaultAsync();

      return Ok(eventList);
    }
  }
}
