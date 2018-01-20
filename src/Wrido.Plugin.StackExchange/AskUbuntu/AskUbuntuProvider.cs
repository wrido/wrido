using System;
using System.Collections.Generic;
using Wrido.Core.QueryLanguage;
using Wrido.Plugin.StackExchange.Common;

namespace Wrido.Plugin.StackExchange.AskUbuntu
{
  public class AskUbuntuProvider : StackExchangeProvider<AskUbuntuResult>
  {
    protected override string Command => ":au";
    protected override string Site => StackExchangeSites.AskUbuntu;

    public AskUbuntuProvider(IStackExchangeClient stackExchangeClient, IQueryParser<SearchQuery> queryParser, IQuestionDescriptionFactory descriptionFactory)
        : base(stackExchangeClient, queryParser, descriptionFactory) { }

    protected override IEnumerable<AskUbuntuResult> CreateFallbackResult(SearchQuery query)
    {
      var url = new Uri($"https://askubuntu.com/search?q={query.InTitle.Replace(' ', '_')}");
      yield return new AskUbuntuResult
      {
        Title = $"Search Ask Ubuntu for '{query.InTitle}'",
        Uri = url,
        Description = url.ToString(),
        Distance = 0,
        Score = 0
      };
    }
  }
}
