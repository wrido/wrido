using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wrido.Logging;

namespace Wrido.Plugin.Wikipedia
{
  public class WikipediaResponseConverter : JsonConverter
  {
    private readonly ILogger _logger;
    private static readonly Type WikiResponseType = typeof(WikipediaResponse);

    public WikipediaResponseConverter(ILogger logger)
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
          _logger.Verbose("Expected term to be string, got {tokenType}", termToken?.Type);
          return null;
        }
        var result = new WikipediaResponse
        {
          Term = termToken.Value<string>()
        };
        _logger.Verbose("Preparing response for search term {searchTerm}", result.Term);

        var titleArray = jArray[1];
        foreach (var titleToken in titleArray)
        {
          result.Suggestions.Add(new WikipediaResponse.WikipediaSuggestion
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
        _logger.Warning(e, "An exception was thrown when creating the WikipediaResponse");
        return null;
      }
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == WikiResponseType;
    }
  }
}
