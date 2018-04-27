using System;
using System.Collections.Concurrent;
using System.Reactive.Subjects;
using Wrido.Queries.Events;

namespace Wrido.Queries
{
  public interface IClientStreamRepository
  {
    IObservable<BackendEvent> GetOrAdd(string connectionId);
    bool TryGetObserver(string connectionId, out IObserver<BackendEvent> obsever);
  }

  public class ClientStreamRepository : IClientStreamRepository
  {
    private readonly ConcurrentDictionary<string, ISubject<BackendEvent>> _subjects;

    public ClientStreamRepository()
    {
      _subjects = new ConcurrentDictionary<string, ISubject<BackendEvent>>();
    }

    public IObservable<BackendEvent> GetOrAdd(string connectionId)
      => _subjects.GetOrAdd(connectionId, id => new Subject<BackendEvent>());
    

    public bool TryGetObserver(string connectionId, out IObserver<BackendEvent> obsever)
    {
      var succecss =_subjects.TryGetValue(connectionId, out var subject);
      if (succecss == false)
      {
        obsever = default;
        return false;
      }
      obsever = subject;
      return true;
    }
  }
}
