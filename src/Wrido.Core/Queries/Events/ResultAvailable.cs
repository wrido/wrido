namespace Wrido.Queries.Events
{
  public class ResultAvailable : BackendEvent
  {
    public ResultAvailable(QueryResult result)
    {
      Result = result;
    }
    public QueryResult Result { get; set; }
  }
}
