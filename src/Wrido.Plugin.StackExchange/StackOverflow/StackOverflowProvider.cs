using System;
using System.Collections.Generic;
using Wrido.Core.QueryLanguage;
using Wrido.Plugin.StackExchange.Common;

namespace Wrido.Plugin.StackExchange.StackOverflow
{
    public class StackOverflowProvider : StackExchangeProvider<StackOverflowResult>
    {
        protected override string Command => ":so";

        public StackOverflowProvider(IStackExchangeClient stackExchangeClient, IQueryParser<SearchQuery> queryParser, IQuestionDescriptionFactory descriptionFactory)
            : base(stackExchangeClient, queryParser, descriptionFactory) { }

        protected override string Site => StackExchangeSites.StackOverflow;

        protected override IEnumerable<StackOverflowResult> CreateFallbackResult(SearchQuery query)
        {
            var url = new Uri($"https://stackoverflow.com/search?q={query.InTitle.Replace(' ', '_')}");
            yield return new StackOverflowResult
            {
                Title = $"Search StackOverflow for '{query.InTitle}'",
                Uri = url,
                Description = url.ToString(),
                Distance = 0,
                Score = 0
            };
        }
    }
}
