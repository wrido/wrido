using System;
using System.Collections.Concurrent;
using System.Reactive.Subjects;
using Wrido.Queries.Events;

namespace Wrido.Queries
{
  public interface IClientStreamRepository
  {
    IObservable<QueryEvent> GetOrAdd(string connectionId);
    bool TryGetObserver(string connectionId, out IObserver<QueryEvent> obsever);
  }

  public class ClientStreamRepository : IClientStreamRepository
  {
    private readonly ConcurrentDictionary<string, ISubject<QueryEvent>> _subjects;

    public ClientStreamRepository()
    {
      _subjects = new ConcurrentDictionary<string, ISubject<QueryEvent>>();
    }

    public IObservable<QueryEvent> GetOrAdd(string connectionId)
      => _subjects.GetOrAdd(connectionId, id => new Subject<QueryEvent>());
    

    public bool TryGetObserver(string connectionId, out IObserver<QueryEvent> obsever)
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
