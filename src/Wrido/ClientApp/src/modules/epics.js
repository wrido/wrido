import {HubConnection} from '@aspnet/signalr';
import {Observable} from 'rxjs';
import * as actions from '../actionCreators';

// TODO: Resolve from config?
const keyMap = {
  nextItem: 'ArrowDown',
  previousItem: 'ArrowUp',
  clearOrHide: 'Escape',
  executeResult: 'Enter',
  togglePreview: 'Tab'
}

export const keyPressEpic = (action$, store) => action$
  .ofType(actions.handleKeyPress.type)
  .map(keyPress => {
    const state = store.getState();

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
      case keyMap.togglePreview:
        return actions.togglePreview();
    }
  })
  .filter(action => action);

export const webSocketEpic = (action$) => {

  const recursiveConnect = (observer) => {
    const connection = new HubConnection('/query');
    connection
      .start()
      .then(() => observer.next(connection));
    connection.onclose(() => recursiveConnect(observer))
  };

  return Observable
    .create(o => recursiveConnect(o))
    .switchMap(connection => {

      const inputChanged$ = action$
        .ofType(actions.onInputChangeAction.type)
        .debounceTime(300)
        .do(action => connection.invoke('QueryAsync', action.value)
        )
        .filter(() => false);

      const invokeSignalR = action$
        .do(action => {
          switch (action.type) {
            case (actions.clearQuery.type):
              connection.invoke('QueryAsync', '');
              break;
            case (actions.hideShell.type):
              connection.invoke('HideShellAsync');
              break;
            case (actions.executeResult.type):
              connection.invoke('ExecuteAsync', action.result);
              break;
          }
        })
        .filter(() => false);

      const backendEvent$ = connection.stream('CreateResponseStream');
      const receiveSignalR = Observable.create(o => backendEvent$.subscribe(o));

      return Observable.merge(invokeSignalR, inputChanged$, receiveSignalR);
    });
}
