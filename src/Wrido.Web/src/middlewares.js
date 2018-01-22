import { onInputChange } from "./constants";

export const connectToSignalR = store => {
  const connection = new window.signalR.HubConnection('/query');

  const log = name => msg => console.log(name, msg);
  const actions = {
    queryReceived: log('queryReceived'),
    queryExecuting: log('queryExecuting'),
    resultsAvailable: log('resultsAvailable'),
    queryCompleted: log('queryCompleted'),
    queryCancelled: log('queryCancelled'),
  }

  connection.start().then(() => {
    Object.keys(actions)
      .map(name => ({ name, action: actions[name] }))
      .forEach(({ name, action }) => connection.on(name, action));
    connection.invoke('QueryAsync', 'google test');
  });
  return next => action => {
    switch (action.type) {
      case onInputChange:
        connection.invoke('QueryAsync', 'google test');
        break;
    }
    return next(action);
  }
}