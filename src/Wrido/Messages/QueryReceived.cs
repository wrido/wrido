using Wrido.Queries;

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
