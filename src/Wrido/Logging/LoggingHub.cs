using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Serilog.Core.Enrichers;

namespace Wrido.Logging
{
  public class LoggingHub : Hub
  {
    private static readonly Serilog.ILogger _fallbackLogger = Log.ForContext<LoggingHub>();
    private static readonly ConcurrentStack<Serilog.ILogger> _loggerStack = new ConcurrentStack<Serilog.ILogger>();

    public void Write(LogLevel level, string messageTemplate, object[] propertyValues)
    {
      var logger = _loggerStack.TryPeek(out var currentLogger) ? currentLogger : _fallbackLogger;
      logger.Write(level.AsSerilogLevel(), (Exception)null, messageTemplate, propertyValues);
    }

    public void BeginScope(string name, object value)
    {
      var propEnricher = new PropertyEnricher(name, value);
      _loggerStack.Push(_loggerStack.TryPeek(out var recentLogger)
        ? recentLogger.ForContext(propEnricher)
        : Log.ForContext(propEnricher));
    }

    public void EndScope()
    {
      _loggerStack.TryPop(out _);
    }
  }
}
