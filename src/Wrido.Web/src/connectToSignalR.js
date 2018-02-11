import { onInputChangeAction } from './actionCreators';
import { isSameActionType } from './reduxUtils';
import { HubConnection } from '@aspnet/signalr-client';

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
      connection.invoke('QueryAsync', action.value)
    }
    return next(action);
  }
}
