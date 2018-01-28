import {
  queryReceivedAction,
  queryExecutingAction,
  resultsAvailableAction,
  queryCompletedAction,
  queryCancelledAction,
  onInputChangeAction
} from './actionCreators';
import { isSameActionType } from './reduxUtils';

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
      .forEach(({ name, action }) => connection.on(name, payload => next(action(payload))));
  });
  return action => {
    if (isSameActionType(action, onInputChangeAction)) {
      connection.invoke('QueryAsync', action.payload.value)
    }
    return next(action);
  }
}
