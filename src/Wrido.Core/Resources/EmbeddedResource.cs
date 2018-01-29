namespace Wrido.Resources
{
  public class EmbeddedResource
  {
    public byte[] Data { get; }
    public string ResourcePath { get; }

    public EmbeddedResource(string resourcePath, byte[] resourceData)
    {
      ResourcePath = resourcePath;
      Data = resourceData;
    }
  }
}
