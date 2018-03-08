using System.Collections.Generic;
using System.Net;
using System.Text;
using Wrido.Plugin.Spotify.Common.Playback;
using Wrido.Plugin.Spotify.Common.RecentlyPlayed;
using Wrido.Plugin.Spotify.Common.Utils;

namespace Wrido.Plugin.Spotify.Common.Search
{
  public interface IQueryParameterBuilder
  {
    string Build(SearchQuery query);
    string Build(RecentlyPlayedQuery query);
    string Build(PlaybackRequest request);
  }

  public class QueryParameterBuilder : IQueryParameterBuilder
  {
    public string Build(SearchQuery query)
    {
      var builder = new StringBuilder("?");
      builder.Append($"q={Encode(query.Query)}");
      builder.Append($"&type={GetTypes(query.Type)}");

      if (!string.IsNullOrEmpty(query.Market))
        builder.Append($"&market={query.Market}");
      if (query.Limit != default)
        builder.Append($"&limit={query.Limit}");
      if (query.Offset != default)
        builder.Append($"&offset={query.Offset}");

      return builder.ToString();
    }

    public string Build(RecentlyPlayedQuery query)
    {
      var builder = new StringBuilder("?");
      var limit = query.Limit == 0 ? 10 : query.Limit;
      builder.Append($"limit={limit}");
      if (query.DateFilter != null)
      {
        switch (query.DateFilter)
        {
          case After after:
            builder.Append($"&after={after.Date.ToEpoch()}");
            break;
          case Before before:
            builder.Append($"&before={before.Date.ToEpoch()}");
            break;
        }
      }

      return builder.ToString();
    }

    public string Build(PlaybackRequest request)
    {
      return string.IsNullOrEmpty(request?.DeviceId)
        ? string.Empty
        : $"?device_id={request?.DeviceId}";
    }

    private string Encode(string query) => WebUtility.UrlEncode(query);

    private string GetTypes(SearchType type)
    {
      var types = new List<string>();
      if (type.HasFlag(SearchType.Artist))
        types.Add("artist");
      if (type.HasFlag(SearchType.Album))
        types.Add("album");
      if (type.HasFlag(SearchType.Playlist))
        types.Add("playlist");
      if (type.HasFlag(SearchType.Track))
        types.Add("track");
      return string.Join(",", types);
    }
  }
}
