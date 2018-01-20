using System.Collections.Generic;
using System.Linq;
using IQueryProvider = Wrido.Core.IQueryProvider;

namespace Wrido.Messages
{
  public class QueryExecuting
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
