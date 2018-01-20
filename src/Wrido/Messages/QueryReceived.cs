namespace Wrido.Messages
{
  public class QueryReceived
  {
    public Core.Query Current { get; }

    public QueryReceived(Core.Query query)
    {
      Current = query;
    }
  }
}
