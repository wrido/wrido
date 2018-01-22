import { onInputChange } from './constants';
import { queryReceivedAction, queryExecutingAction, resultsAvailableAction, queryCompletedAction, queryCancelledAction } from './actionCreators';

const actions = {
  queryReceived: queryReceivedAction,
  queryExecuting: queryExecutingAction,
  resultsAvailable: resultsAvailableAction,
  queryCompleted: queryCompletedAction,
  queryCancelled: queryCancelledAction,
}

export const connectToSignalR = store => next => {
  const connection = new window.signalR.HubConnection('/query');
  connection.start().then(() => {
    Object.keys(actions)
      .map(name => ({ name, action: actions[name] }))
      .forEach(({ name, action }) => connection.on(name, data => next(action(data))));
  });
  return action => {
    if (action.type === onInputChange) {
      connection.invoke('QueryAsync', action.payload.value)
    }
    return next(action);
  }
}