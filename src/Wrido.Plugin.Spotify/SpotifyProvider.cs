using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Plugin.Spotify.Authorization;
using Wrido.Plugin.Spotify.Common;
using Wrido.Plugin.Spotify.Common.RecentlyPlayed;
using Wrido.Plugin.Spotify.Common.Search;
using Wrido.Plugin.Spotify.Playback;
using Wrido.Queries;

namespace Wrido.Plugin.Spotify
{
  public class SpotifyProvider : QueryProvider
  {
    private readonly SpotifyConfiguration _config;
    private readonly ISpotifyClient _client;

    public SpotifyProvider(SpotifyConfiguration config, ISpotifyClient client)
    {
      _config = config;
      _client = client;
    }

    public override bool CanHandle(Query query)
    {
      if (query?.Command == null)
      {
        return false;
      }
      return query.Command.Equals(_config.Keyword, StringComparison.InvariantCultureIgnoreCase);
    }

    protected override async Task QueryAsync(Query query, CancellationToken ct)
    {
      if (!_client.CanAuthenticate)
      {
        Available(new AuthorizationRequiredResult
        {
          Title = "Authentication required",
          Description = "You need to give Wrido right to access you account"
        });
        return;
      }

      if (string.IsNullOrWhiteSpace(query.Argument))
      {
        var currentPlayback = new SpotifyTogglePlaybackResult
        {
          Title = "Loading current play state..."
        };
        Available(currentPlayback);

        var foreverTask = Task.Run(async () =>
        {
          while (true)
          {
            var playback = await _client.GetCurrentPlaybackAsync(ct);
            if (playback == null)
            {
              currentPlayback.Title = "Nothing playing";
            }
            else
            {
              var duration = TimeSpan.FromMilliseconds(playback.ProgressMs ?? 0);
              var action = playback.IsPlaying ? "Pause" : "Play";
              currentPlayback.IsPlaying = playback.IsPlaying;
              currentPlayback.Title = $"{action} '{playback.Item.Name}' on {playback.Device?.Name}";
              currentPlayback.Description = $"{duration:mm\\:ss}";
            }

            Updated(currentPlayback);
            await Task.Delay(TimeSpan.FromSeconds(1), ct);
          }
        }, ct);

        var recentlyPlayed = await _client.GetRecentlyPlayedAsync(RecentlyPlayedQuery.Default, ct);
        foreach (var played in recentlyPlayed.Items)
        {
          if (string.Equals(played.Context?.Type, "playlist"))
          {
            // Hack, reported at: https://github.com/spotify/web-api/issues/815
            played.Context.Uri = null;
          }

          Available(new SpotifyPlayableResult
          {
            Title = $"'{played.Track.Name}' by {played.Track.Artists.FirstOrDefault()?.Name}",
            Description = $"Last played {played.PlayedAt}",
            ResourceUri = played.Track.Uri,
            ContextUri = played.Context?.Uri
          });
        }

        await foreverTask;
      }

      var search = await _client.SearchAsync(new SearchQuery{ Query = query.Argument, Type = SearchType.All, Limit = 5}, ct);
      foreach (var track in search.Tracks.Items)
      {
        Available(new SpotifyPlayableResult
        {
          Title = $"Track '{track.Name}' by {track.Artists.FirstOrDefault()?.Name}",
          Description = $"From {track.Album.Name}, {track.Album.ReleaseDate}",
          ResourceUri = track.Uri,
          ContextUri = track.Album?.Uri
        });
      }

      foreach (var artist in search.Artists.Items)
      {
        Available(new SpotifyPlayableResult
        {
          Title = $"Top songs for '{artist.Name}'",
          Description = string.Join(" ", artist.Genres),
          ContextUri = artist.Uri
        });
      }

      foreach (var album in search.Albums.Items)
      {
        var albumResult = new SpotifyPlayableResult
        {
          Title = $"Album '{album.Name}' by {album.Artists.FirstOrDefault()?.Name}",
          Description = $"{album.AlbumType}, {album.ReleaseDate}",
          ContextUri = album.Uri
        };
        Available(albumResult);
      }
    }
  }
}
