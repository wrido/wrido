using System;

namespace Wrido.Logging
{
  public static class LoggerExtensions
  {
    public static void Verbose(this ILogger logger, string messageTemplate, params object[] propertyValues) =>
      logger.Verbose(null, messageTemplate, propertyValues);

    public static void Verbose(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues) =>
      logger.Write(LogLevel.Verbose, exception, messageTemplate, propertyValues);

    public static void Debug(this ILogger logger, string messageTemplate, params object[] propertyValues) =>
      logger.Debug(null, messageTemplate, propertyValues);

    public static void Debug(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues) =>
      logger.Write(LogLevel.Debug, exception, messageTemplate, propertyValues);

    public static void Information(this ILogger logger, string messageTemplate, params object[] propertyValues) =>
      logger.Information(null, messageTemplate, propertyValues);

    public static void Information(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues) =>
      logger.Write(LogLevel.Information, exception, messageTemplate, propertyValues);

    public static void Warning(this ILogger logger, string messageTemplate, params object[] propertyValues) =>
      logger.Warning(null, messageTemplate, propertyValues);

    public static void Warning(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues) =>
      logger.Write(LogLevel.Warning, exception, messageTemplate, propertyValues);

    public static void Error(this ILogger logger, string messageTemplate, params object[] propertyValues) =>
      logger.Error(null, messageTemplate, propertyValues);

    public static void Error(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues) =>
      logger.Write(LogLevel.Error, exception, messageTemplate, propertyValues);

    public static void Critical(this ILogger logger, string messageTemplate, params object[] propertyValues) =>
      logger.Critical(null, messageTemplate, propertyValues);

    public static void Critical(this ILogger logger, Exception exception, string messageTemplate, params object[] propertyValues) =>
      logger.Write(LogLevel.Critical, exception, messageTemplate, propertyValues);
  }
}
