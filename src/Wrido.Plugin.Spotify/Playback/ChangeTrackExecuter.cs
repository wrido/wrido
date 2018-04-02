using System.Collections.Generic;
using System.Linq;
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
      return result is IPlayableResource;
    }

    public async Task ExecuteAsync(QueryResult result)
    {
      if (!(result is IPlayableResource playable))
      {
        return;
      }

      var req = new PlaybackRequest();
      if (string.IsNullOrEmpty(playable.ContextUri))
      {
        req.Uris = playable.ResourceUris;
      }
      else
      {
        req.ContextUri = playable.ContextUri;
        if (playable.ResourceUris == null)
        {
          req.Offset = new PositionOffset(1);
        }
        else
        {
          req.Offset = new UriOffset(playable.ResourceUris.First());
        }
      }

      await _spotifyClient.PlayAsync(req);
    }
  }

  public interface IPlayableResource
  {
    IEnumerable<string> ResourceUris { get; }
    string ContextUri { get; }
  }
}
