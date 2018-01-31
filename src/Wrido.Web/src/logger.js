export const logger = store => next => action => {
  const before = store.getState();
  const result = next(action);
  const after = store.getState();
  console.group(`Action dispatched: ${action.type}`);
  console.log('Before', before);
  console.log('Action', action);
  console.log('After', after);
  console.groupEnd();
  return result
}
