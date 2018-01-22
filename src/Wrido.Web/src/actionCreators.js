import { onInputChange, queryCancelled, queryCompleted, queryExecuting, queryReceived, resultsAvailable } from './constants';

const createActionCreator = (type, payloader) => payload => ({ type, payload: payloader(payload) });

export const onInputChangeAction = createActionCreator(onInputChange, value => ({ value }));
export const queryCancelledAction = createActionCreator(queryCancelled, value => ({ value }));
export const queryCompletedAction = createActionCreator(queryCompleted, value => ({ value }));
export const queryExecutingAction = createActionCreator(queryExecuting, value => ({ value }));
export const queryReceivedAction = createActionCreator(queryReceived, value => ({ value }));
export const resultsAvailableAction = createActionCreator(resultsAvailable, value => ({ value }));