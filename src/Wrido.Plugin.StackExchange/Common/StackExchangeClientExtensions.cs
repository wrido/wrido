using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wrido.Plugin.StackExchange.Common
{
    public static class StackExchangeClientExtensions
    {
        public static Task<IList<Question>> SearchAsync(this IStackExchangeClient client, string site, string inTitle, CancellationToken ct = default)
        {
            return client.SearchAsync(new SearchQuery
            {
                InTitle = inTitle,
                Site = site
            }, ct);
        }
    }
}
