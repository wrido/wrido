namespace Wrido.Queries.Events
{
  public class ResultUpdated : BackendEvent
  {
    public QueryResult Result { get; set; }

    public ResultUpdated(QueryResult updatedResult)
    {
      Result = updatedResult;
    }
  }
}
