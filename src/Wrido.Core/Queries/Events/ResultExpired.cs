using System;

namespace Wrido.Queries.Events
{
  public class ResultExpired : BackendEvent
  {
    public Guid ResultId { get; set; }
  }
}
