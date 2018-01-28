using System.Linq;
using Autofac;
using Autofac.Core;
using Serilog;

namespace Wrido.Logging
{
  public class LoggingModule : Module
  {
    protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
    {
      // Ignore components that provide loggers
      if (registration.Services.OfType<TypedService>().Any(ts => ts.ServiceType == typeof(ILogger)))
      {
        return;
      }

      registration.Preparing += (sender, args) =>
      {
        var serilogLogger = Log.ForContext(registration.Activator.LimitType);
        ILogger wridoLogger = new SerilogLogger(serilogLogger);
        args.Parameters = new[] { TypedParameter.From(wridoLogger) }.Concat(args.Parameters);
      };
    }
  }
}
