using System;
using Wrido.Plugin.StackExchange.Common;
using Xunit;

namespace Wrido.Plugin.StackExchange.Tests
{
    public class QuestionDescriptionFactoryTests
    {
        [Fact]
        public void Should_Resolve_To_Years_If_Created_One_Year_Ago()
        {
            // Given
            var created = DateTime.Now.AddYears(-1);

            // When
            var friendlyDate = QuestionDescriptionFactory.GetUserFriendlyDate(created);

            // Then
            Assert.Equal("12 months ago", friendlyDate);
        }

        [Fact]
        public void Should_Use_Months_When_Less_Than_Two_Years_Ago()
        {
            // Given
            var created = DateTime.Now.AddYears(-1).AddMonths(-7);

            // When
            var friendlyDate = QuestionDescriptionFactory.GetUserFriendlyDate(created);

            // Then
            Assert.Equal("19 months ago", friendlyDate);
        }

        [Fact]
        public void Should_Resolve_To_Years_If_Created_More_Than_One_Year_Ago()
        {
            // Given
            var created = DateTime.Now.AddYears(-10);

            // When
            var friendlyDate = QuestionDescriptionFactory.GetUserFriendlyDate(created);

            // Then
            Assert.Equal("10 years ago", friendlyDate);
        }
    }
}
