namespace Wrido.Queries.Events
{
  public class ResultUpdated : QueryEvent
  {
    public QueryResult Result { get; set; }

    public ResultUpdated(QueryResult updatedResult)
    {
      Result = updatedResult;
    }
  }
}
