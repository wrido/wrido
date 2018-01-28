using System.Diagnostics;
using Wrido.Core.Queries;

namespace Wrido.Queries
{
  [DebuggerDisplay("{Title} {Uri}")]
  public class WebResult : QueryResult
  {
    public System.Uri Uri { get; set; }
  }
}