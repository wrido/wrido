using System.Threading.Tasks;

namespace Wrido.Plugin.Spotify.Authorization
{
  public interface IAccessTokenProvider
  {
    Task<string> GetAsync();
    bool IsReady { get; }
  }
}
