using System;
using System.Linq;

namespace Wrido.Queries
{
  public interface IQuery
  {
    string Command { get; }
    string Argument { get; }
    string Raw { get; }
    Guid Id { get; }
  }

  public class Query : IQuery
  {
    public string Command { get; }
    public string Argument { get; }
    public string Raw { get; }
    public Guid Id { get; }

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

  public class DefaultQuery : IQuery
  {
    public string Command => string.Empty;
    public string Argument { get; }
    public string Raw { get; }
    public Guid Id { get; }

    public DefaultQuery(IQuery query)
    {
      Argument = query.Raw;
      Raw = query.Raw;
      Id = query.Id;
    }
  }
}
