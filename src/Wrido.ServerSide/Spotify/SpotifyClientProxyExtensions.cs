using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Wrido.ServerSide.Spotify
{
  public static class SpotifyClientProxyExtensions
  {
    private const string authorizeCallback = "authorizeUrlAvailable";
    private const string authorizeFailed = "authorizeFailed";
    private const string authorizeSucceeded = "authorizeSucceeded";

    public static Task SendAuthorizationUrl(this IClientProxy client, string url) =>
      client.SendAsync(authorizeCallback, url);

    public static Task SendAccessObjectAsync(this IClientProxy client, SpotifyAccess access) =>
      client.SendAsync(authorizeSucceeded, access);

    public static Task NotifyAuthorizationError(this IClientProxy client, string error) =>
      client.SendAsync(authorizeFailed, error);
  }
}
