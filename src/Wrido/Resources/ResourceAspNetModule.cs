using Autofac;
using Microsoft.AspNetCore.StaticFiles;

namespace Wrido.Resources
{
  public class ResourceAspNetModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder
        .RegisterType<FileExtensionContentTypeProvider>()
        .AsImplementedInterfaces();
    }
  }
}
