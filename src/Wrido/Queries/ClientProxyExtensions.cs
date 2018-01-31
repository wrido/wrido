using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Wrido.Messages;

namespace Wrido.Queries
{
  public static class ClientProxyExtensions
  {
    private const string _eventMethod = "event";

    public static Task InvokeAsync<TMessage>(this IClientProxy client, TMessage message, CancellationToken ct = default) where TMessage : MessageBase
    {
      return message == null
        ? Task.FromCanceled(ct)
        : client.InvokeAsync(_eventMethod, message);
    }
  }
}
