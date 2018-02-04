using System.Collections.Generic;
using Wrido.Configuration;

namespace Wrido.Plugin.Wikipedia
{
  public class WikipediaConfiguration : IPluginConfiguration
  {
    public IList<string> BaseUrls { get; set; }
    public string Keyword { get; set; }

    public static WikipediaConfiguration Fallback = new WikipediaConfiguration
    {
      Keyword = ":wiki",
      BaseUrls = new List<string>
      {
        "https://en.wikipedia.org"
      }
    };
  }
}
