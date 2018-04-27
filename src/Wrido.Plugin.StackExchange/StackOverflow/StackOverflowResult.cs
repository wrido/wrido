using System;
using Wrido.Resources;

namespace Wrido.Plugin.StackExchange.StackOverflow
{
  public class StackOverflowResult : StackExchangeResult
  {
    private static readonly Image _stackOverflowIcon = new Image
    {
      Uri = new Uri("/resources/wrido/plugin/stackexchange/resources/StackOverflow.png", UriKind.Relative),
      Alt = "StackOverflow"
    };

    private const string _stackOverflow = "StackOverflow";

    public StackOverflowResult()
    {
      Icon = _stackOverflowIcon;
      Category = _stackOverflow;
    }
  }
}
