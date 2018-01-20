using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;
using Wrido.Core;
using Wrido.Core.QueryLanguage;
using Wrido.Plugin.StackExchange.Common;
using Wrido.Plugin.StackExchange.StackOverflow;
using Xunit;

namespace Wrido.Plugin.StackExchange.Tests
{
    public class StackOverflowProviderTests
    {
        [Fact]
        public async Task Should_Handle_Simple_Queries_As_Text_In_Question_Title()
        {
            // Given
            const string simpleQuery = "so pattern matching";

            var stackExchangeClient = Substitute.For<IStackExchangeClient>();
            var queryParser = new QueryParser<SearchQuery>();
            var descriptionFactory = Substitute.For<IQuestionDescriptionFactory>();
            var provider = new StackOverflowProvider(stackExchangeClient, queryParser, descriptionFactory);

            stackExchangeClient
                .SearchAsync(Arg.Any<SearchQuery>(), CancellationToken.None)
                .Returns(Task.FromResult<IList<Question>>(new List<Question>()));

            var query = new Query(simpleQuery);

            // When
            await provider.QueryAsync(query, CancellationToken.None);

            // Then
            var stackExchangeQuery = stackExchangeClient.ReceivedCalls().First().GetArguments()[0] as SearchQuery;
            Assert.NotNull(stackExchangeQuery);
            Assert.Equal(stackExchangeQuery.InTitle, "pattern matching");
        }

        [Fact]
        public async Task Should_Handle_Queries_With_Free_Text_And_Expressions()
        {
            // Given
            const string simpleQuery = "so pattern matching tagged:c#";

            var stackExchangeClient = Substitute.For<IStackExchangeClient>();
            var queryParser = new QueryParser<SearchQuery>();
            var descriptionFactory = Substitute.For<IQuestionDescriptionFactory>();
            var provider = new StackOverflowProvider(stackExchangeClient, queryParser, descriptionFactory);

            stackExchangeClient
                .SearchAsync(Arg.Any<SearchQuery>(), CancellationToken.None)
                .Returns(Task.FromResult<IList<Question>>(new List<Question>()));

            var query = new Query(simpleQuery);

            // When
            await provider.QueryAsync(query, CancellationToken.None);

            // Then
            var stackExchangeQuery = stackExchangeClient.ReceivedCalls().First().GetArguments()[0] as SearchQuery;
            Assert.NotNull(stackExchangeQuery);
            Assert.Equal(stackExchangeQuery.InTitle, "pattern matching");
            Assert.Contains("c#", stackExchangeQuery.Tagged);
        }
    }
}
