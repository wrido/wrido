using System.Threading.Tasks;
using Wrido.Execution;
using Wrido.Plugin.Spotify.Common;
using Wrido.Plugin.Spotify.Common.Playback;
using Wrido.Queries;

namespace Wrido.Plugin.Spotify.Playback
{
  public class PlayPauseExecutor : IResultExecuter
  {
    private readonly ISpotifyClient _client;

    public PlayPauseExecutor(ISpotifyClient client)
    {
      _client = client;
    }

    public bool CanExecute(QueryResult result)
    {
      return result is SpotifyTogglePlaybackResult;
    }

    public async Task ExecuteAsync(QueryResult result)
    {
      if (!(result is SpotifyTogglePlaybackResult toggle))
      {
        return;
      }

      if (toggle.IsPlaying)
      {
        await _client.PauseAsync();
      }
      else
      {
        await _client.PlayAsync(new PlaybackRequest());
      }
    }
  }
}
