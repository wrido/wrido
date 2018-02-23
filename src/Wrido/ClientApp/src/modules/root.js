import { combineEpics } from 'redux-observable';
import { combineReducers } from 'redux';
import { input, result } from './reducers';
import { backendStreamEpic, inputEpic} from "./epics";

export const rootEpic = combineEpics(
  backendStreamEpic,
  inputEpic
);

export const rootReducer = combineReducers({
  input,
  result
})
