using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Wrido.Messages;

namespace Wrido.Queries
{
  public interface IExecutionService
  {
    Task ExecuteAsync(IClientProxy client, QueryResult result);
  }

  public class ExecutionService : IExecutionService
  {
    private readonly IEnumerable<IResultExecuter> _executors;

    public ExecutionService(IEnumerable<IResultExecuter> executors)
    {
      _executors = executors;
    }

    public async Task ExecuteAsync(IClientProxy client, QueryResult result)
    {
      var executor = _executors.FirstOrDefault(e => e.CanExecute(result));
      if (executor == null)
      {
        await client.InvokeAsync(new ExecutionFailed { Reason = "Can not execute"});
        return;
      }
      await executor.ExecuteAsync(result);
      await client.InvokeAsync(new ExecutionCompleted());
    }
  }
}
