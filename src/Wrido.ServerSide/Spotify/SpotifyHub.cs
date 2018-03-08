using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Internal;

namespace Wrido.ServerSide.Spotify
{
  public class SpotifyHub : Hub
  {
    private readonly SpotifyOptions _options;

    public SpotifyHub(SpotifyOptions options)
    {
      _options = options;
    }

    public async Task StartAuthorizationAsync()
    {
      var clientId = _options.ClientId;
      var state = Context.ConnectionId;
      var redirectUri = _options.AuthorizeRedirectUrl;
      var scope = new[]
      {
        Scopes.UserReadRecentlyPlayed,
        Scopes.PlaylistReadPrivate,
        Scopes.PlaylistReadCollaborative,
        Scopes.UserReadPlaybackState,
        Scopes.UserModifyPlaybackState
      }.Join("%20");
      var authorizeUrl =$"{_options.AuthorizeUrl}?client_id={clientId}&scope={scope}&state={state}&redirect_uri={redirectUri}&response_type=code";
      await Clients.Caller.SendAuthorizationUrl(authorizeUrl);
    }
  }
}
