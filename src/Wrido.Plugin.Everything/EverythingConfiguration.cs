using Wrido.Configuration;

namespace Wrido.Plugin.Everything
{
  public class EverythingConfiguration : IPluginConfiguration
  {
    public string Keyword { get; private set; }

    public static EverythingConfiguration Default => new EverythingConfiguration
    {
      Keyword = ":e"
    };
  }
}
