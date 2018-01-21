using System.Collections.Generic;
using Wrido.Resources;

namespace Wrido.Core.Queries
{
  public class QueryResult
  {
    public string Title { get; set; }
    public string Description { get; set; }
    public IEnumerable<Resource> Resources { get; set; }
  }
}