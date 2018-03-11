using System.Threading.Tasks;
using Wrido.Queries;

namespace Wrido.Execution
{
  public interface IResultExecuter
  {
    bool CanExecute(QueryResult result);
    Task ExecuteAsync(QueryResult result);
  }
}