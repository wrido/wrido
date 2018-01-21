using System;

namespace Wrido.Logging
{
  public interface ILogger
  {
    void Write(LogLevel level, Exception exception, string messageTemplate, params object[] propertyValues);
    IDisposable BeginScope(string name, object value);
    bool IsEnabled(LogLevel level);
  }
}
