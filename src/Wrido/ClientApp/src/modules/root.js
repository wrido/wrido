import { combineEpics } from 'redux-observable';
import { combineReducers } from 'redux';
import { input, result } from './reducers';
import { keyPressEpic} from './epics';

export const rootEpic = combineEpics(
  keyPressEpic
);

export const rootReducer = combineReducers({
  input,
  result
})
