using System;
using Wrido.Core;

namespace Wrido.Plugin.StackExchange
{
  public abstract class StackExchangeResult : QueryResult
  {
    public Uri Uri { get; internal set; }
    public float Distance { get; internal set; }
    public float Score { get; internal set; }
  }
}
