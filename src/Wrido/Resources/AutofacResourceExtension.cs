using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;

namespace Wrido.Resources
{
  public static class AutofacResourceExtension
  {
    public static ContainerBuilder RegisterResources<TAssemblyType>(this ContainerBuilder builder)
    {
      return builder.RegisterResources(typeof(TAssemblyType).Assembly);
    }

    public static ContainerBuilder RegisterResources(this ContainerBuilder builder, object instanceFromAssembly)
    {
      return builder.RegisterResources(instanceFromAssembly.GetType().Assembly);
    }

    public static ContainerBuilder RegisterResources(this ContainerBuilder builder, Assembly assembly)
    {
      var allResoures = assembly.GetManifestResourceNames();

      foreach (var resourceName in allResoures)
      {
        var resourcePath = CreateResourcePath(resourceName);

        using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
        using (var memoryStream = new MemoryStream())
        {
          try
          {
            resourceStream.CopyTo(memoryStream);
            var resource = new EmbeddedResource(resourcePath, memoryStream.GetBuffer());
            builder
              .Register(c => resource)
              .As<EmbeddedResource>()
              .Named<EmbeddedResource>(resourceName)
              .SingleInstance();
          }
          catch (Exception) { }
        }
      }
      return builder;
    }

    private static string CreateResourcePath(string resourceName)
    {
      var parts = resourceName.Split('.');
      var fileExtension = parts.Last();
      var pathWithoutExtension = parts.Take(parts.Length -1).Aggregate((agg, delta) => $"{agg}/{delta}");
      return $"{pathWithoutExtension}.{fileExtension}";
    }
  }
}
