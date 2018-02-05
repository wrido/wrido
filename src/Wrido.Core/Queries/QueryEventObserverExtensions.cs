using System;
using Wrido.Queries.Events;

namespace Wrido.Queries
{
  public static class QueryEventObserverExtensions
  {
    public static void ResultAvailable(this IObserver<QueryEvent> observer, QueryResult result, Guid queryId)
    {
      observer?.OnNext(new ResultAvailable(result, queryId));
    }

    public static void ResultUpdated(this IObserver<QueryEvent> observer, QueryResult result)
    {
      observer?.OnNext(new ResultUpdated(result));
    }
  }
}
