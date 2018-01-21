using System;

namespace Wrido.Messages
{
  public class ExecutionFailed
  {
    public Guid Id { get; set; }
    public string Reason { get; set; }
  }
}
