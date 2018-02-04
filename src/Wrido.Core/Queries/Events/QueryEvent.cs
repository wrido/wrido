namespace Wrido.Queries.Events
{
  public abstract class QueryEvent
  {
    protected QueryEvent()
    {
      Type = GetType().Name;
    }

    public string Type { get; }
  }
}
