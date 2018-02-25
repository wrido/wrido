import { onInputChangeAction, clearQuery, hideShell, executeResult } from './actionCreators';
import { isSameActionType } from './reduxUtils';
import { HubConnection } from '@aspnet/signalr';

export const connectToSignalR = store => next => {
  const connection = new HubConnection('/query');
  connection
    .start()
    .then(() => {
      connection
        .stream('CreateResponseStream')
        .subscribe({
          close: false,
          next: msg => next(msg),
          error: err => console.log(err),
          complete: () => console.log('completed')
      });
    });
  return action => {
    if (isSameActionType(action, onInputChangeAction)) {
      connection.invoke('QueryAsync', action.value);
    }
    else if(isSameActionType(action, clearQuery)){
      connection.invoke('QueryAsync', '');
    }
    else if(isSameActionType(action, hideShell)){
      connection.invoke('HideShellAsync');
    }
    else if(isSameActionType(action, executeResult)){
      connection.invoke('ExecuteAsync', action.result)
    }
    return next(action);
  }
}
