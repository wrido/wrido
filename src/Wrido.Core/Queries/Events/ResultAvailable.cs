namespace Wrido.Queries.Events
{
  public class ResultAvailable : QueryEvent
  {
    public ResultAvailable(QueryResult result)
    {
      Result = result;
    }
    public QueryResult Result { get; set; }
  }
}
