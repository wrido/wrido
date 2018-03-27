using System;
using System.Threading.Tasks;
using Everything.LowLevel.SyntacticSugar;
using Wrido.Execution;
using Wrido.Queries;

namespace Wrido.Plugin.Everything
{
  public class EverythingExecutor : IResultExecuter
  {
    public bool CanExecute(QueryResult result)
    {
      return result is EverythingResult;
    }

    public Task ExecuteAsync(QueryResult result)
    {
      if (!(result is EverythingResult everything))
      {
        throw new ArgumentException("Expected an EverythingResult", nameof(result));
      }

      EverythingSdk.IncrementRunCount(everything.FullPath);
      OpenDefault.PathOrUrl(everything.FullPath);
      return Task.CompletedTask;
    }
  }
}
