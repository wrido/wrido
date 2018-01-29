export const logger = store => next => action => {
  console.group(`Action dispatched: ${action.type}`);
  console.log('Action', action);
  console.log('State', store.getState());
  console.groupEnd();
  return next(action);
}
