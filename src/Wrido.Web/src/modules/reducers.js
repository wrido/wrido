import { onInputChangeAction, queryReceivedAction, resultsAvailableAction } from '../actionCreators';
import { reducer } from '../reduxUtils';

export const input = reducer(
  { value: '' },
  [onInputChangeAction, (state, action) => ({ value: action.payload.value })]
);

export const result = reducer(
  { currentQueryId: null, items: [] },
  [
    queryReceivedAction,
    (state, action) => ({
      currentQueryId: action.payload.value.current.id,
      items: []
    })
  ],
  [
    resultsAvailableAction,
    (state, action) => ({
      items: action.payload.value.queryId === state.currentQueryId ?
        state.items.concat(action.payload.value.results.$values) :
        state.items
    })
  ],
);
