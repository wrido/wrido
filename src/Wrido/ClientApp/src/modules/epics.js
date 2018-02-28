import {HubConnection} from '@aspnet/signalr';
import {Observable} from 'rxjs';
import * as actions from '../actionCreators';

// TODO: Resolve from config?
const keyMap = {
  nextItem: 'ArrowDown',
  previousItem: 'ArrowUp',
  clearOrHide: 'Escape',
  executeResult: 'Enter'
}

export const keyPressEpic = (action$, store) => action$
  .ofType(actions.handleKeyPress.type)
  .map(keyPress => {
    let state = store.getState();

    switch (keyPress.key) {
      case keyMap.nextItem:
        return actions.selectNextResult();
      case keyMap.previousItem:
        return actions.selectPreviousResult();
      case keyMap.clearOrHide:
        return state.input.value
          ? actions.clearQuery()
          : actions.hideShell();
      case keyMap.executeResult:
        if (state.result.active) {
          return actions.executeResult(state.result.active);
        }
    }
  })
  .filter(action => action);

export const webSocketEpic = (action$) => {

  const recursiveConnect = (observer) => {
    let connection = new HubConnection('/query');
    connection
      .start()
      .then(() => observer.next(connection));
    connection.onclose(() => recursiveConnect(observer))
  };

  return Observable
    .create(o => recursiveConnect(o))
    .switchMap(connection => {
      var invokeSignalR = action$
        .do(action => {
          switch (action.type) {
            case (actions.onInputChangeAction.type):
              connection.invoke('QueryAsync', action.value);
              break;
            case (actions.clearQuery.type):
              connection.invoke('QueryAsync', '');
              break;
            case (actions.hideShell):
              connection.invoke('HideShellAsync');
              break;
            case (actions.executeResult):
              connection.invoke('ExecuteAsync', action.result);
              break;
            default:
              break;
          }
        })
        .filter(() => false);

      let backendEvent$ = connection.stream('CreateResponseStream');
      let receiveSignalR = Observable.create(o => backendEvent$.subscribe(o));

      return Observable.merge(invokeSignalR, receiveSignalR);
    });
}
