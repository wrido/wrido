using System.Collections.Generic;
using Wrido.Configuration;

namespace Wrido.Plugin.Wikipedia
{
  public class WikipediaPluginConfiguration : IPluginConfiguration
  {
    public IList<string> BaseUrls { get; set; }
    public string Keyword { get; set; }

    public static WikipediaPluginConfiguration Fallback = new WikipediaPluginConfiguration
    {
      Keyword = ":wiki",
      BaseUrls = new List<string>
      {
        "https://en.wikipedia.org"
      }
    };
  }
}
