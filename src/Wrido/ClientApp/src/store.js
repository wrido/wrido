import { createStore, applyMiddleware } from 'redux';
import { rootReducer } from './modules/root';
import { createEpicMiddleware } from 'redux-observable';
import { rootEpic } from './modules/root';
import { createLogger } from 'redux-logger';

const middleware = [
  createEpicMiddleware(rootEpic),
  createLogger(),
];

const store = createStore(
  rootReducer,
  applyMiddleware(...middleware)
);

export default store