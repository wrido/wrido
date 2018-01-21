using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NSubstitute;
using Wrido.Logging;
using Wrido.Plugin.StackExchange.Common;
using Wrido.TestCommon.Fakes;
using Wrido.TestCommon.Utilities;
using Xunit;

namespace Wrido.Plugin.StackExchange.Tests
{
    public class StackExchangeClientTests
    {
        [Fact]
        public async Task Should_Throw_Argument_Exception_If_Both_InTitle_And_Tag_Are_Not_Set()
        {
            // Given
            var httpClient = Substitute.For<HttpClient>();
            var queryBuilder = Substitute.For<IQueryStringBuilder>();
            var serializer = Substitute.For<JsonSerializer>();
            var logger = Substitute.For<ILogger>();

            var client = new StackExchangeClient(httpClient, queryBuilder, serializer, logger);
            var searchQuery = new SearchQuery
            {
                InTitle = null,
                Tagged = Enumerable.Empty<string>()
            };

            // When
            // Then
            await Assert.ThrowsAsync<ArgumentException>(() => client.SearchAsync(searchQuery));
        }

        [Fact]
        public async Task Should_Send_Http_Request_With_Query_Parameter_From_Provider()
        {
            // Given
            var searchQuery = new SearchQuery { InTitle = "Wrido" };
            var expectedQueryParamter = $"/id={Guid.NewGuid()}";

            var httpHandler = Substitute.ForPartsOf<FakeHttpHandler>();
            var queryBuilder = Substitute.For<IQueryStringBuilder>();
            var serializer = Substitute.For<JsonSerializer>();
            var logger = Substitute.For<ILogger>();

            queryBuilder
                .Build(searchQuery)
                .Returns(expectedQueryParamter);

            httpHandler
                .Send(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(string.Empty) });

            var client = new StackExchangeClient(new HttpClient(httpHandler) { BaseAddress = new Uri("http://localhost") }, queryBuilder, serializer, logger);

            // When
            await client.SearchAsync(searchQuery);

            // Then
            var outgoingRequest = httpHandler.ReceivedCalls().First().GetArguments()[0] as HttpRequestMessage;
            Assert.NotNull(outgoingRequest);
            Assert.Equal(outgoingRequest.RequestUri.PathAndQuery, expectedQueryParamter);
        }

        [Fact]
        public void Should_Not_Send_Http_Request_If_Cancellation_Is_Requested()
        {
            // Given
            var searchQuery = new SearchQuery { InTitle = "Wrido" };

            var httpHandler = Substitute.ForPartsOf<FakeHttpHandler>();
            var queryBuilder = Substitute.For<IQueryStringBuilder>();
            var serializer = Substitute.For<JsonSerializer>();
            var logger = Substitute.For<ILogger>();

            httpHandler.Send(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>());

            var client = new StackExchangeClient(new HttpClient(httpHandler) { BaseAddress = new Uri("http://localhost") }, queryBuilder, serializer, logger);

            // When
            client
                .SearchAsync(searchQuery, new CancellationToken(canceled: true))
                .ContinueWith(t =>
                {
                    // Then
                    Assert.True(t.IsCanceled);
                    Assert.Empty(httpHandler.ReceivedCalls());
                })
                .GetAwaiter()
                .GetResult();
        }

        [Fact]
        public async Task Should_Return_Questions_Retrieved_From_Api_Endpoint()
        {
            // Given
            var questions = new PaginationResult<Question>
            {
                Items = new List<Question>
                {
                    new Question { Title = "Regexp for match second numeric values" },
                    new Question { Title = "How to use regexp for finding patterns in strings?" }
                },
            };
            var httpHandler = Substitute.ForPartsOf<FakeHttpHandler>();
            var queryBuilder = Substitute.For<IQueryStringBuilder>();
            var serializer = new JsonSerializer();
            var logger = Substitute.For<ILogger>();

            queryBuilder
                .Build(Arg.Any<SearchQuery>())
                .Returns(string.Empty);

            httpHandler
                .Send(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
                .Returns(new HttpResponseMessage(HttpStatusCode.OK) { Content = new JsonContent(questions, serializer) });

            var client = new StackExchangeClient(new HttpClient(httpHandler) { BaseAddress = new Uri("http://localhost") }, queryBuilder, serializer, logger);

            // When
            var result = await client.SearchAsync(new SearchQuery { InTitle = "Wrido" });

            // Then
            Assert.Equal(questions.Items[0].Title, result[0].Title);
            Assert.Equal(questions.Items[1].Title, result[1].Title);
        }
    }
}
