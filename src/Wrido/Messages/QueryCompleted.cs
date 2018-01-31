using System;
using System.Collections.Generic;
using System.Linq;
using Wrido.Queries;

namespace Wrido.Messages
{
  public sealed class QueryCompleted : MessageBase
  {
    public QueryCompleted(Guid queryId, IEnumerable<QueryResult> results)
    {
      QueryId = queryId;
      Results = results.ToList().AsReadOnly();
    }

    public Guid QueryId { get; }
    public IList<QueryResult> Results { get; }
  }
}
