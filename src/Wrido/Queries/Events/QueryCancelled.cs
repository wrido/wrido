using System;

namespace Wrido.Queries.Events
{
  public class QueryCancelled : QueryEvent
  {
    public QueryCancelled(Guid queryId)
    {
      QueryId = queryId;
    }

    public Guid QueryId { get; set; }
  }
}
