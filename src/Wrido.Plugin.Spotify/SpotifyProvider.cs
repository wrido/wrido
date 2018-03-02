using System;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Plugin.Spotify.Common;
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
      return query.Command.Equals(_config.Keyword, StringComparison.InvariantCultureIgnoreCase);
    }

    protected override async Task QueryAsync(Query query, CancellationToken ct)
    {
      if (!_client.IsAuthenticated)
      {
        Available(new SpotifyAuthRequiredResult
        {
          Title = "Authentication required",
          Description = "You need to give Wrido right to access you account"
        });
        return;
      }
      throw new System.NotImplementedException();
    }
  }
}
