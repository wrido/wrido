using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Wrido.Query
{
  public static class ClientProxyExtensions
  {
    public static Task InvokeAsync<TMessage>(this IClientProxy client, TMessage message, CancellationToken ct = default) where TMessage : class
    {
      return message == null
        ? Task.FromCanceled(ct)
        : client.InvokeAsync(typeof(TMessage).Name, message);
    }
  }
}
