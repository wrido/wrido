using System;
using Wrido.Queries;
using Wrido.Resources;

namespace Wrido.Plugin.Everything
{
  public class EverythingResult : QueryResult
  {
    private static readonly Image everythingLogo = new Image
    {
      Uri = new Uri("/resources/wrido/plugin/everything/resources/everything.png", UriKind.Relative),
      Alt = "Everything",
    };

    public EverythingResult()
    {
      Icon = everythingLogo;
    }

    public string FullPath { get; set; }
  }
}
