namespace Wrido.Queries.Events
{
  public class QueryReceived : BackendEvent
  {
    public IQuery Current { get; }

    public QueryReceived(IQuery query)
    {
      Current = query;
    }
  }
}
