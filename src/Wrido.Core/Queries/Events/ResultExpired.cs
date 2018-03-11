using System;

namespace Wrido.Queries.Events
{
  public class ResultExpired : QueryEvent
  {
    public Guid ResultId { get; set; }
  }
}
