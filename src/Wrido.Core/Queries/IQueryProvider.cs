using System;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Queries.Events;

namespace Wrido.Queries
{
  public interface IQueryProvider
  {
    bool CanHandle(Query query);
    Task QueryAsync(Query query, IObserver<QueryEvent> observer, CancellationToken ct);
  }
}
