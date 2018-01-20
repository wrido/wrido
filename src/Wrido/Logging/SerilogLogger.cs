using System;
using Serilog.Context;
using Wrido.Core.Logging;

namespace Wrido.Logging
{
  public class SerilogLogger : ILogger
  {
    private readonly Serilog.ILogger _logger;

    public SerilogLogger(Serilog.ILogger logger)
    {
      _logger = logger;
    }

    public void Write(LogLevel level, Exception exception, string messageTemplate, params object[] propertyValues)
    {
      _logger.Write(level.AsSerilogLevel(), exception, messageTemplate, propertyValues);
    }

    public IDisposable BeginScope(string name, object value)
    {
      return LogContext.PushProperty(name, value);
    }

    public bool IsEnabled(LogLevel level)
    {
      return _logger.IsEnabled(level.AsSerilogLevel());
    }
  }
}
