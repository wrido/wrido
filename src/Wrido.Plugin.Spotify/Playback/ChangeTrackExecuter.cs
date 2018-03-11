using System.Threading.Tasks;
using Wrido.Execution;
using Wrido.Plugin.Spotify.Common;
using Wrido.Plugin.Spotify.Common.Playback;
using Wrido.Queries;

namespace Wrido.Plugin.Spotify.Playback
{
  public class ChangeTrackExecuter : IResultExecuter
  {
    private readonly ISpotifyClient _spotifyClient;

    public ChangeTrackExecuter(ISpotifyClient spotifyClient)
    {
      _spotifyClient = spotifyClient;
    }

    public bool CanExecute(QueryResult result)
    {
      return result is SpotifyPlayableResult;
    }

    public async Task ExecuteAsync(QueryResult result)
    {
      if (!(result is SpotifyPlayableResult playable))
      {
        return;
      }

      var req = new PlaybackRequest();
      if (string.IsNullOrEmpty(playable.ContextUri))
      {
        req.Uris = new[] {playable.ResourceUri};
      }
      else
      {
        req.ContextUri = playable.ContextUri;
        if (playable.ResourceUri == null)
        {
          req.Offset = new PositionOffset(1);
        }
        else
        {
          req.Offset = new UriOffset(playable.ResourceUri);
        }
      }

      await _spotifyClient.PlayAsync(req);
    }
  }
}
