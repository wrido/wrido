using System;
using System.Linq;

namespace Wrido.Core
{
  public class Query
  {
    public string Command { get; }
    public string Argument { get; }
    public string Raw { get; }
    public Guid Id { get; set; }

    public Query(string query)
    {
      Raw = query;
      Id = Guid.NewGuid();

      var parts = Raw?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
      Command = parts?.FirstOrDefault();
      Argument = string.Join(" ", parts?.Skip(1) ?? Enumerable.Empty<string>()).Trim();
    }

    public override string ToString() => Raw;
  }
}
