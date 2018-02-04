using System.Collections.Generic;
using System.Linq;

namespace Wrido.Queries.Events
{
  public class QueryExecuting : QueryEvent
  {
    public IList<string> Providers { get; set; }

    public static QueryExecuting By(IEnumerable<IQueryProvider> providers)
    {
      return new QueryExecuting
      {
        Providers = providers.Select(p => p.GetType().Name).ToList()
      };
    }
  }
}
