using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Wrido")]

namespace Wrido.Logging
{
  public class LogManager
  {
    internal static Func<Type, ILogger> LoggerFactory;
    private static readonly SilentLogger Silent = new SilentLogger();

    public static ILogger GetLogger<TLogger>()
    {
      return LoggerFactory?.Invoke(typeof(TLogger)) ?? Silent;
    }

    private class SilentLogger : ILogger, IDisposable
    {
      public void Write(LogLevel level, Exception exception, string messageTemplate, params object[] propertyValues) { }
      public IDisposable BeginScope(string name, object value) => this;
      public bool IsEnabled(LogLevel level) => false;
      public void Dispose() { }
    }
  }
}
