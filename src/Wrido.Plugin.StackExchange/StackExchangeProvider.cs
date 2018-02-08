using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Plugin.StackExchange.Common;
using Wrido.Queries;

namespace Wrido.Plugin.StackExchange
{
  public abstract class StackExchangeProvider<TQueryResult> : QueryProvider
      where TQueryResult : StackExchangeResult, new()
  {
    private readonly IStackExchangeClient _stackExchangeClient;
    private readonly IQueryParser<SearchQuery> _queryParser;
    private readonly IQuestionDescriptionFactory _descriptionFactory;
    
    protected abstract string Site { get; }
    protected abstract string Command { get; }

    protected StackExchangeProvider(IStackExchangeClient stackExchangeClient, IQueryParser<SearchQuery> queryParser, IQuestionDescriptionFactory descriptionFactory)
    {
      _stackExchangeClient = stackExchangeClient;
      _queryParser = queryParser;
      _descriptionFactory = descriptionFactory;
    }

    public override bool CanHandle(Query query)
    {
      if (string.IsNullOrEmpty(query.Argument))
      {
        return false;
      }
      return string.Equals(query.Command, Command, StringComparison.OrdinalIgnoreCase);
    }

    protected override async Task QueryAsync(Query query, CancellationToken ct)
    {
      var searchQuery = _queryParser.Bind(query.Argument, out var freeText);
      searchQuery.Site = Site;
      if (string.IsNullOrEmpty(searchQuery.InTitle))
      {
        searchQuery.InTitle = freeText;
      }
      var questions = await _stackExchangeClient.SearchAsync(searchQuery, ct);
      if (!questions.Any())
      {
        foreach (var queryResult in CreateFallbackResult(searchQuery))
        {
          Available(queryResult);
        }
        return;
      }
      foreach (var queryResult in questions.Select(q => ConvertQuestion(q, query)))
      {
        Available(queryResult);
      }
    }

    protected abstract IEnumerable<TQueryResult> CreateFallbackResult(SearchQuery query);

    protected virtual TQueryResult ConvertQuestion(Question question, Query query)
    {
      return new TQueryResult
      {
        Title = question.Title,
        Description = _descriptionFactory.Create(question),
        Uri = question.Link,
      };
    }
  }
}
