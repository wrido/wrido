using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Plugin.Spotify.Authorization;
using Wrido.Plugin.Spotify.Common;
using Wrido.Plugin.Spotify.Common.Model;
using Wrido.Plugin.Spotify.Common.Model.Full;
using Wrido.Plugin.Spotify.Common.RecentlyPlayed;
using Wrido.Plugin.Spotify.Common.Search;
using Wrido.Plugin.Spotify.Playback;
using Wrido.Plugin.Spotify.Resources;
using Wrido.Queries;

namespace Wrido.Plugin.Spotify
{
  public class SpotifyProvider : QueryProvider
  {
    private readonly SpotifyConfiguration _config;
    private readonly ISpotifyClient _client;
    private readonly WridoAccessTokenProvider _accessTokenProvider;

    public SpotifyProvider(SpotifyConfiguration config, ISpotifyClient client, WridoAccessTokenProvider accessTokenProvider)
    {
      _config = config;
      _client = client;
      _accessTokenProvider = accessTokenProvider;
    }

    public override bool CanHandle(IQuery query)
    {
      if (query?.Command == null)
      {
        return false;
      }
      return query.Command.Equals(_config.Keyword, StringComparison.InvariantCultureIgnoreCase);
    }

    protected override async Task QueryAsync(IQuery query, CancellationToken ct)
    {
      if (!_accessTokenProvider.IsReady)
      {
        await PromptForAuthorizationAsync(ct);
      }

      var isDefault = query is DefaultQuery;

      if (!string.IsNullOrWhiteSpace(query.Argument))
      {
        var numberOfResults = (ushort)(isDefault ? 1 : 5);
        await SearchForTrackAsync(query.Argument, numberOfResults, ct);
        return;
      }

      if (isDefault)
      {
        return;
      }

      var foreverStreamTask = StreamCurrentlyPlayingAsync(ct);
      await LoadRecentlyPlayedAsync(ct);
      await foreverStreamTask;
    }

    private async Task PromptForAuthorizationAsync(CancellationToken ct)
    {
      var authResult = new AuthorizationRequiredResult
      {
        Title = "Authentication required",
        Description = "You need to give Wrido right to access you account"
      };

      Available(authResult);
      await _accessTokenProvider.WaitUntilReadyAsync(ct);
      Expired(authResult);
    }

    private async Task LoadRecentlyPlayedAsync(CancellationToken ct)
    {
      var recentlyPlayed = await _client.GetRecentlyPlayedAsync(RecentlyPlayedQuery.Default, ct);
      var playedByContext = recentlyPlayed.Items.GroupBy(i => i.Context?.Uri);

      foreach (var playedItems in playedByContext)
      {
        var playedResult = playedItems
          .Select(played => new PlayableAlbumResult
          {
            Title = $"'{played.Track.Name}' by {played.Track.Artists.FirstOrDefault()?.Name}",
            Description = $"Last played {played.PlayedAt}",
            ResourceUris = new[] {played.Track.Uri},
            ContextUri = played.Context?.Uri,
            PreviewUri = PreviewUri.Track,
            ArtistName = played.Track.Artists.FirstOrDefault()?.Name,
            ActiveTrackName = played.Track.Name
          })
          .ToList();

        Available(playedResult);

        var sharedContext = playedItems.FirstOrDefault().Context;
        if (sharedContext == null)
        {
          continue;
        }

        if (sharedContext.IsAlbum())
        {
          // Bug reported at https://github.com/spotify/web-api/issues/836
          if (sharedContext.Href.PathAndQuery.EndsWith("/null"))
          {
            var albumId = sharedContext.Uri.Split(':').LastOrDefault();
            sharedContext.Href = new Uri($"{sharedContext.Href.Scheme}://{sharedContext.Href.Host}{sharedContext.Href.PathAndQuery.Replace("/null", $"/{albumId}")}");
          }

          var album = await _client.GetAsync<Album>(sharedContext.Href.ToString(), ct);
          foreach (var result in playedResult)
          {
            result.AlbumName = album.Name;
            result.CoverArt = album.Images.FirstOrDefault(i => i.Width == 300)?.Url ??
                              album.Images.LastOrDefault()?.Url;
            result.ReleaseDate = album.ReleaseDate;
            Updated(result);
          }
        } else if (sharedContext.IsPlaylist())
        {
          var playlist = await _client.GetAsync<Playlist>(sharedContext.Href.ToString(), ct);
          foreach (var result in playedResult)
          {
            result.AlbumName = playlist.Name;
            result.CoverArt = playlist.Images.FirstOrDefault()?.Url;
            result.ReleaseDate = playlist.Owner.Id;
            Updated(result);
          }
        }
      }
    }

    private async Task StreamCurrentlyPlayingAsync(CancellationToken ct)
    {
      var currentPlayback = new SpotifyTogglePlaybackResult
      {
        Title = "Loading current play state..."
      };

      Available(currentPlayback);

      while (true)
      {
        var playback = await _client.GetCurrentPlaybackAsync(ct);
        if (playback == null)
        {
          currentPlayback.Title = "Nothing playing";
        }
        else
        {
          var progress = TimeSpan.FromMilliseconds(playback.ProgressMs ?? 0);
          var duration = TimeSpan.FromMilliseconds(playback.Item.DurationMs);
          var action = playback.IsPlaying ? "Pause" : "Play";
          currentPlayback.IsPlaying = playback.IsPlaying;
          currentPlayback.Title = $"{action} '{playback.Item.Name}' on {playback.Device?.Name}";
          currentPlayback.Description = $"{progress:mm\\:ss}";
          currentPlayback.PlaybackProgress = $"{progress:mm\\:ss}";
          currentPlayback.TrackDuration = $"{duration:mm\\:ss}";
          currentPlayback.AlbumName = playback.Item.Album.Name;
          currentPlayback.ArtistName = playback.Item.Artists.FirstOrDefault()?.Name;
          currentPlayback.ReleaseDate = playback.Item.Album.ReleaseDate;
          currentPlayback.ActiveTrackName = playback.Item.Name;
          currentPlayback.CoverArt = playback.Item.Album.Images.FirstOrDefault(i => i.Width == 300)?.Url ??
                                     playback.Item.Album.Images.FirstOrDefault()?.Url;
          currentPlayback.PreviewUri = PreviewUri.NowPlaying;
        }

        Updated(currentPlayback);
        await Task.Delay(TimeSpan.FromSeconds(1), ct);
        ct.ThrowIfCancellationRequested();
      }
    }

    public async Task SearchForTrackAsync(string query, ushort limit, CancellationToken ct)
    {
      var search = await _client.SearchAsync(new SearchQuery { Query = query, Type = SearchType.All, Limit = limit }, ct);
      ct.ThrowIfCancellationRequested();

      var trackResults = search.Tracks.Items.Select(track => new PlayableAlbumResult
      {
        Title = $"{track.Artists.FirstOrDefault()?.Name} - {track.Name}",
        Description = $"{track.Album.Name}, {track.Album.ReleaseDate}",
        ResourceUris = new[] { track.Uri },
        ContextUri = track.Album?.Uri,
        CoverArt = track.Album?.Images.Skip(1).FirstOrDefault().Url,
        ArtistName = track.Artists.FirstOrDefault()?.Name,
        ActiveTrackName = track.Name,
        AlbumName = track.Album.Name,
        ReleaseDate = track.Album.ReleaseDate,
        PreviewUri = PreviewUri.Track
      }).ToList();

      Available(trackResults);

      foreach (var album in search.Albums.Items)
      {
        var albumResult = new PlayableAlbumResult
        {
          Title = $"{album.Artists.FirstOrDefault()?.Name} - {album.Name}",
          Description = $"{album.AlbumType}, {album.ReleaseDate}",
          ContextUri = album.Uri,
          AlbumName = album.Name,
          ReleaseDate = album.ReleaseDate,
          ArtistName = album.Artists.FirstOrDefault()?.Name,
          CoverArt = album.Images.Skip(1).FirstOrDefault().Url,
          PreviewUri = PreviewUri.Album
        };
        Available(albumResult);
      }

      foreach (var artist in search.Artists.Items)
      {
        const string tempCountryCode = "se";
        var topTracks = await _client.GetTopTracks(artist.Id, tempCountryCode, ct);
        Available(new PlayableAlbumResult
        {
          Title = $"{artist.Name} - Top tracks",
          Description = $"{topTracks.Tracks[0].Name} and 9 more",
          CoverArt = artist.Images.FirstOrDefault(i => i.Width == 300)?.Url ?? artist.Images.LastOrDefault()?.Url,
          ResourceUris = topTracks.Tracks.Select(t => t.Uri).ToList(),
          PreviewUri = PreviewUri.Artist,
          AlbumName = "Top tracks",
          ArtistName = artist.Name,
        });
      }
    }
  }
}
