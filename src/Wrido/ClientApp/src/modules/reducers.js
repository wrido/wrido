import { onInputChangeAction, queryReceivedAction, resultAvailableAction } from '../actionCreators';
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
    resultAvailableAction,
    (state, action) => ({
      items: state.items.concat(action.result)
    })
  ],
);
