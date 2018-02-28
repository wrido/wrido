import { combineEpics } from 'redux-observable';
import { combineReducers } from 'redux';
import { input, result } from './reducers';
import { keyPressEpic, webSocketEpic} from './epics';

export const rootEpic = combineEpics(
  keyPressEpic,
  webSocketEpic,
);

export const rootReducer = combineReducers({
  input,
  result
})
