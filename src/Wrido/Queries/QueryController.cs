using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wrido.Queries.Events;

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
      var events = new List<QueryEvent>();
      var accumulater = Observer.Create<QueryEvent>(@event => events.Add(@event));
      await _queryService.QueryAsync(rawQuery, accumulater);
      return Ok(events);
    }
  }
}
