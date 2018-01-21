using System;
using System.Collections.Generic;
using System.IO;

namespace Wrido.Core.Resources
{
  public class EmbeddedResource
  {
    public byte[] Data { get; }
    public string ResourcePath { get; }
    public string ContentType { get; }

    private static readonly Dictionary<string, string> ContetTypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
    {
      {".png", "image/png"},
      {".jpeg", "image/jpeg"},
      {".gif", "image/gif"},
    };

    public EmbeddedResource(string resourcePath, byte[] resourceData)
    {
      ResourcePath = resourcePath;
      Data = resourceData;
      var fileExtension = Path.GetExtension(resourcePath);
      if (ContetTypes.ContainsKey(fileExtension))
      {
        ContentType = ContetTypes[fileExtension];
      }
    }
  }
}
