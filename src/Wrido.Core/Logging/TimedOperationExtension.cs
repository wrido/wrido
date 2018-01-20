namespace Wrido.Core.Logging
{
  public static class TimedOperationExtensions
  {
    public static TimedOperation Timed(this ILogger logger, string messageTemplate,
        params object[] propertyValues)
    {
      return logger.Timed(LogLevel.Information, messageTemplate, propertyValues);
    }

    public static TimedOperation Timed(this ILogger logger, LogLevel level, string messageTemplate,
        params object[] propertyValues)
    {
      return logger.Timed(level, level, messageTemplate, propertyValues);
    }

    public static TimedOperation Timed(this ILogger logger, LogLevel completionLevel, LogLevel cancelledLevel, string messageTemplate,
        params object[] propertyValues)
    {
      return new TimedOperation(logger, messageTemplate, propertyValues, completionLevel, cancelledLevel);
    }
  }
}
