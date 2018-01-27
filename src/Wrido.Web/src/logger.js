export const logger = store => next => action => {
  console.log(`Action dispatched: ${action.type}`, action)
  return next(action);
}
