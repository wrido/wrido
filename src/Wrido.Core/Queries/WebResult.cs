using System.Diagnostics;

namespace Wrido.Queries
{
  [DebuggerDisplay("{Title} {Uri}")]
  public class WebResult : QueryResult
  {
    public System.Uri Uri { get; set; }
    public System.Uri PreviewUri { get; set; }
  }
}