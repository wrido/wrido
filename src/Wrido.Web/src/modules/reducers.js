import { onInputChangeAction, queryReceivedAction, resultsAvailableAction } from '../actionCreators';
import { reducer } from '../reduxUtils';

export const input = reducer(
  { value: '' },
  [onInputChangeAction, (state, action) => ({ value: action.value })]
);

export const result = reducer(
  { currentQueryId: null, items: [] },
  [
    queryReceivedAction,
    (state, action) => ({
      currentQueryId: action.current.id,
      items: []
    })
  ],
  [
    resultsAvailableAction,
    (state, action) => ({
      items: action.queryId === state.currentQueryId ?
        state.items.concat(action.results.$values) :
        state.items
    })
  ],
);
