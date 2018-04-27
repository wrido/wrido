using System;
using Wrido.Resources;

namespace Wrido.Plugin.StackExchange.AskUbuntu
{
  public class AskUbuntuResult : StackExchangeResult
  {
    private static readonly Image _askUbuntuIcon = new Image
    {
      Uri = new Uri("/resources/wrido/plugin/stackexchange/resources/AskUbuntu.png", UriKind.Relative),
      Alt = "StackOverflow"
    };

    private const string _askUbuntu = "Ask Ubuntu";

    public AskUbuntuResult()
    {
      Icon = _askUbuntuIcon;
      Category = _askUbuntu;
    }
  }
}
