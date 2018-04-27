namespace Wrido.Queries.Events
{
  public abstract class BackendEvent
  {
    protected BackendEvent()
    {
      Type = GetType().Name;
    }

    public string Type { get; }
  }
}
