using System;

namespace Wrido.Queries.Events
{
  public class ExecutionFailed : QueryEvent
  {
    public Guid Id { get; set; }
    public string Reason { get; set; }
  }
}
