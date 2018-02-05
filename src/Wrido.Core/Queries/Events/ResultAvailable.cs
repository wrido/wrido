using System;

namespace Wrido.Queries.Events
{
  public class ResultAvailable : QueryEvent
  {
    public QueryResult Result { get; set; }
    public Guid QueryId { get; set; }

    public ResultAvailable(QueryResult result, Guid queryId)
    {
      Result = result;
      QueryId = queryId;
    }
  }
}
