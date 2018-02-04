using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wrido.Logging;
using Wrido.Plugin.Wikipedia.Common;

namespace Wrido.Plugin.Wikipedia.Serialization
{
  public class WikipediaSearchConverter : JsonConverter
  {
    private readonly ILogger _logger;
    private static readonly Type WikiResponseType = typeof(SearchResult);

    public WikipediaSearchConverter(ILogger logger)
    {
      _logger = logger;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      try
      {
        var jArray = JArray.Load(reader);
        var termToken = jArray?[0];
        if (termToken?.Type != JTokenType.String)
        {
          LoggerExtensions.Verbose(_logger, "Expected term to be string, got {tokenType}", termToken?.Type);
          return null;
        }
        var result = new SearchResult
        {
          Term = termToken.Value<string>()
        };
        LoggerExtensions.Verbose(_logger, "Preparing response for search term {searchTerm}", result.Term);

        var titleArray = jArray[1];
        foreach (var titleToken in titleArray)
        {
          result.Suggestions.Add(new SearchResult.WikipediaSuggestion
          {
            Title = titleToken.Value<string>()
          });
        }
        var descriptionArray = jArray[2];
        for (var i = 0; i < descriptionArray.Count(); i++)
        {
          result.Suggestions[i].Description = descriptionArray[i].Value<string>();
        }
        var uriArray = jArray[3];
        for (var i = 0; i < uriArray.Count(); i++)
        {
          result.Suggestions[i].Uri = new Uri(uriArray[i].Value<string>());
        }

        return result;
      }
      catch (Exception e)
      {
        LoggerExtensions.Warning(_logger, e, "An exception was thrown when creating the SearchResult");
        return null;
      }
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == WikiResponseType;
    }
  }
}
