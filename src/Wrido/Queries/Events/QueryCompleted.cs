using System;
using System.Collections.Generic;
using System.Linq;

namespace Wrido.Queries.Events
{
  public sealed class QueryCompleted : QueryEvent
  {
    public QueryCompleted(Guid queryId, IEnumerable<QueryResult> results)
    {
      QueryId = queryId;
      Results = results?.ToList().AsReadOnly();
    }

    public Guid QueryId { get; }
    public IList<QueryResult> Results { get; }
  }
}
