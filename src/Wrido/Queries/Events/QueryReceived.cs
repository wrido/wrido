namespace Wrido.Queries.Events
{
  public class QueryReceived : QueryEvent
  {
    public Query Current { get; }

    public QueryReceived(Query query)
    {
      Current = query;
    }
  }
}
