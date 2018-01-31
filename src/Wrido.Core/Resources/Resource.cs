using System;

namespace Wrido.Resources
{
  public abstract class Resource
  {
  }

  public sealed class Image : Resource
  {
    public Uri Uri { get; set; }
    public string Alt { get; set; }
  }

  public sealed class Script : Resource
  {
    public Script(string uri)
    {
      if (Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out var parsed))
      {
        Uri = parsed;
      }
    }

    public Uri Uri { get; set; }
  }
}
