using System;
using System.Diagnostics;
using System.Linq;

namespace Wrido.Logging
{
  public class TimedOperation : IDisposable
  {
    private readonly ILogger _logger;
    private readonly string _messageTemplate;
    private readonly object[] _propertyValues;
    private readonly long _startTime;
    private long _stopTime;
    private readonly IDisposable _operationScope;

    public bool IsCompleted { get; private set; }
    public bool IsCancelled { get; private set; }
    public LogLevel CompletionLevel { get; }
    public LogLevel CancelledLevel { get; }
    public Guid OperationId { get; }
    public TimeSpan Elapsed => IsCompleted ? new TimeSpan(_stopTime - _startTime) : new TimeSpan(Stopwatch.GetTimestamp() - _startTime);

    public TimedOperation(ILogger logger, string messageTemplate, object[] propertyValues, LogLevel completionLevel, LogLevel? cancelledLevel = null)
    {
      _logger = logger;
      _messageTemplate = messageTemplate;
      _propertyValues = propertyValues;
      CompletionLevel = completionLevel;
      CancelledLevel = cancelledLevel ?? completionLevel;
      OperationId = Guid.NewGuid();
      _operationScope = logger.BeginScope("operationId", OperationId);
      _startTime = Stopwatch.GetTimestamp();
    }

    public void Cancel()
    {
      IsCancelled = true;
      Stop();
    }

    public void Complete()
    {
      Stop();
    }

    public void Dispose()
    {
      if (IsCompleted)
      {
        return;
      }
      Stop();
    }

    private void Stop()
    {
      _stopTime = Stopwatch.GetTimestamp();
      IsCompleted = true;
      Write();
      _operationScope.Dispose();
    }

    private void Write()
    {
      string outcome;
      LogLevel logLevel;
      if (IsCancelled)
      {
        outcome = "cancelled";
        logLevel = CancelledLevel;
      }
      else
      {
        outcome = "completed";
        logLevel = CompletionLevel;
      }
      var finalMsgTempalte = $"{_messageTemplate} {{outcome:l}} in {{elapsed:0.0}} ms.";
      var finalPropertyValues = _propertyValues
        .Concat(new object[] { outcome, Elapsed.TotalMilliseconds })
        .ToArray();
      _logger.Write(logLevel, null, finalMsgTempalte, finalPropertyValues);
    }
  }
}
