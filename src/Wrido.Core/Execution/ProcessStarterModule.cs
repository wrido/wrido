using System;
using System.Runtime.InteropServices;
using Autofac;

namespace Wrido.Execution
{
  public class ProcessStarterModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder
        .Register<IProcessStarter>(context =>
        {
          if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
          {
            return new WindowsProcessStarter();
          }

          if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
          {
            return new MacProcessStarter();
          }

          if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
          {
            return new LinuxProcessStarter();
          }
          throw new PlatformNotSupportedException();
        })
        .SingleInstance();
    }
  }
}