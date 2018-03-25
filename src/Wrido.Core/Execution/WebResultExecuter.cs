using System.Threading;
using System.Threading.Tasks;
using Wrido.Queries;

namespace Wrido.Execution
{
  public class WebResultExecuter : IResultExecuter
  {
    public bool CanExecute(QueryResult result)
    {
      return result is WebResult;
    }

    public Task ExecuteAsync(QueryResult result)
    {
      if (result is WebResult webResult)
      {
        OpenDefault.PathOrUrl(webResult.Uri.AbsoluteUri);
        return Task.CompletedTask;
      }
      return Task.FromCanceled(CancellationToken.None);
    }
  }
}
