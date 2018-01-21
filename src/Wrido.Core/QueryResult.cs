using System.Collections.Generic;
using Wrido.Core.Resources;

namespace Wrido.Core
{
  public class QueryResult
  {
    public string Title { get; set; }
    public string Description { get; set; }
    public IEnumerable<Resource> Resources { get; set; }
  }
}