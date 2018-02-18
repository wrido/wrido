using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Wrido.Queries.Events;

namespace Wrido.Queries
{
  public static class ClientProxyExtensions
  {
    private const string _eventMethod = "event";

    public static Task SendAsync<TMessage>(this IClientProxy client, TMessage message, CancellationToken ct = default) where TMessage : QueryEvent
    {
      return message == null
        ? Task.FromCanceled(ct)
        : client.SendAsync(_eventMethod, message);
    }
  }
}
