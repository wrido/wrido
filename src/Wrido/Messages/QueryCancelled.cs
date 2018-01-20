using System;

namespace Wrido.Messages
{
  public class QueryCancelled
  {
    public QueryCancelled(Guid queryId)
    {
      QueryId = queryId;
    }

    public Guid QueryId { get; set; }
  }
}
