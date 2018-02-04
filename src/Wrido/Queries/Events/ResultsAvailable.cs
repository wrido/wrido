using System;
using System.Collections.Generic;
using System.Linq;

namespace Wrido.Queries.Events
{
  [Obsolete("Use the 'ResultAvailable' class instead")]
  public class ResultsAvailable : QueryEvent
  {
    public ResultsAvailable(Guid queryId, IEnumerable<QueryResult> results)
    {
      QueryId = queryId;
      Results = results.ToList().AsReadOnly();
    }

    public Guid QueryId { get; }
    public IList<QueryResult> Results { get; }
  }
}
