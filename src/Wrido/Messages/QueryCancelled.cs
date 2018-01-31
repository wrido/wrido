using System;

namespace Wrido.Messages
{
  public class QueryCancelled : MessageBase
  {
    public QueryCancelled(Guid queryId)
    {
      QueryId = queryId;
    }

    public Guid QueryId { get; set; }
  }
}
