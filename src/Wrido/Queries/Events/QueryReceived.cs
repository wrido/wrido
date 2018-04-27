namespace Wrido.Queries.Events
{
  public class QueryReceived : QueryEvent
  {
    public IQuery Current { get; }

    public QueryReceived(IQuery query)
    {
      Current = query;
    }
  }
}
