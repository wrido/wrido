using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Core.Queries;

namespace Wrido.Queries
{
  public interface IQueryProvider
  {
    bool CanHandle(Query query);

    Task<IEnumerable<QueryResult>> QueryAsync(Query query, CancellationToken ct);
  }
}
