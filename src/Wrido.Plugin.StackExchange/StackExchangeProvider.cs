using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wrido.Plugin.StackExchange.Common;
using Wrido.Queries;
using Wrido.Queries.Events;

namespace Wrido.Plugin.StackExchange
{
  public abstract class StackExchangeProvider<TQueryResult> : Queries.IQueryProvider
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

    public bool CanHandle(Query query)
    {
      return string.Equals(query.Command, Command, StringComparison.OrdinalIgnoreCase);
    }

    public async Task QueryAsync(Query query, IObserver<QueryEvent> observer, CancellationToken ct)
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
          observer.ResultAvailable(queryResult, query.Id);
        }
        return;
      }
      foreach (var queryResult in questions.Select(q=> ConvertQuestion(q, query)))
      {
        observer.ResultAvailable(queryResult, query.Id);
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
