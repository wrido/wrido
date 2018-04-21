using System;
using Wrido.Resources;

namespace Wrido.Queries
{
  public class QueryResult
  {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public Image Icon { get; set; }
    public Uri PreviewUri { get; set; }

    public QueryResult()
    {
      Id = Guid.NewGuid();
    }
  }
}