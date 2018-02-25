using System;
using Wrido.Resources;

namespace Wrido.Plugin.StackExchange.StackOverflow
{
  public class StackOverflowResult : StackExchangeResult
  {
    private static readonly Image _wikiLogo = new Image
    {
      Uri = new Uri("/resources/wrido/plugin/stackexchange/resources/StackOverflow.png", UriKind.Relative),
      Alt = "StackOverflow"
    };

    public StackOverflowResult()
    {
      Icon = _wikiLogo;
    }
  }
}
