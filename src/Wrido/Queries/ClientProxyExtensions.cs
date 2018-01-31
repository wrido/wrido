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
      if (message == null)
      {
        return Task.FromCanceled(ct);
      }
      return Task.WhenAll(
        client.InvokeAsync(_eventMethod, message),

        // TODO: remove this call when front end uses on 'event'
        client.InvokeAsync(typeof(TMessage).Name, message)
      );
    }
  }
}
