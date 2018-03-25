using Wrido.Queries;

namespace Wrido.Plugin.Everything
{
  public class EverythingNotAvailableResult : QueryResult
  {
    public EverythingNotAvailableResult(string reason)
    {
      Icon = EverythingIcon.EverythingLogo;
      Title = reason;
    }
  }
}
