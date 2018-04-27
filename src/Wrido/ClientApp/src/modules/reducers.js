import * as action from '../actionCreators';
import {reducer} from '../reduxUtils';

export const input = reducer(
  {value: ''},
  [action.onInputChangeAction, (state, action) => ({value: action.value})],
  [action.clearQuery, (state, action) => ({value: ''})]
);

export const result = reducer(
  {currentQueryId: null, active: null, previewEnabled: true, items: []},
  [
    action.queryReceivedAction,
    (state, action) => ({
      currentQueryId: action.current.id,
      items: [],
      active: null
    })
  ],
  [ action.clearQuery, () => ({items: [], active: null}) ],
  [
    action.resultAvailableAction,
    (state, action) => ({
      items: state.items.concat(action.result),
      active : state.active ? state.active : action.result
    })
  ],
  [
    action.resultUpdated,
    (state, action) => ({
      items: state.items.map(i => i.id === action.result.id ? action.result : i),
      active: (state.active && state.active.id === action.result.id) ? action.result : state.active
    })
  ],
  [
    action.resultExpired,
    (state, action) => ({items: state.items.filter(i => i.id !== action.resultId)})
  ],
  [
    action.selectNextResult,
    (state) => {
      if (!state.items)
        return;
      if (!state.active)
        return {active: state.items[0]};
      let currentIndex = state.items.indexOf(state.active);
      if (currentIndex === state.items.length - 1)
        return state;
      return {active: state.items[currentIndex + 1]}
    }
  ],
  [
    action.selectPreviousResult,
    (state) => {
      if (!state.items)
        return;
      if (!state.active)
        return {active: state.items[0]};
      let currentIndex = state.items.indexOf(state.active);
      if (currentIndex === 0)
        return;
      return {active: state.items[currentIndex - 1]}
    }
  ],
  [action.togglePreview, (state) => ({previewEnabled: !state.previewEnabled})],
);
