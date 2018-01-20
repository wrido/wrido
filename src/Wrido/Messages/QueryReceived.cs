using Wrido.Core;

namespace Wrido.Messages
{
  public class QueryReceived
  {
    public Query Current { get; }

    public QueryReceived(Query query)
    {
      Current = query;
    }
  }
}
