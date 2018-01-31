using System;
using System.Collections.Generic;
using System.Linq;
using Wrido.Queries;

namespace Wrido.Messages
{
    public class ResultsAvailable : MessageBase
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
