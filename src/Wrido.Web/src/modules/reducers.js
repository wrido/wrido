import { queryReceived, resultsAvailable, onInputChange } from '../constants';

const reducer = (initialState, ...configs) => (state = initialState, action) =>
  configs
    .filter(config => config[0] === action.type)
    .reduce((state, config) => ({
      ...state,
      ...config[1](state, action)
    }), state);

export const input = reducer(
  { value: 'success' },
  [onInputChange, (state, action) => ({ value: action.payload.value })]
);

export const result = reducer(
  { currentQueryId: null, items: [] },
  [
    queryReceived,
    (state, action) => ({
      currentQueryId: action.payload.value.current.id,
      items: []
    })
  ],
  [
    resultsAvailable,
    (state, action) => ({
      items: action.payload.value.queryId === state.currentQueryId ? action.payload.value.results.$values : state.items
    })
  ],
);
