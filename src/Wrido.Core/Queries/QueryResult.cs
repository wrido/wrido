using Wrido.Resources;

namespace Wrido.Queries
{
  public class QueryResult
  {
    public string Title { get; set; }
    public string Description { get; set; }
    public Image Icon { get; set; }
    public Script Renderer { get; set; }
  }
}