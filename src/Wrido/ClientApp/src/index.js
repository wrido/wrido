import 'rxjs';
import React from 'react';
import { Provider } from 'react-redux';
import { render } from 'react-dom';
import App from './App';
import store from './store';
import {prepareRenderers} from './actionCreators';

fetch('/resources?type=js')
  .then(scriptSrcs => {
    store.dispatch(prepareRenderers(scriptSrcs))
  })
  .catch(reason => console.warn(reason));

render(
    <Provider store={store}>
        <App />
    </Provider>,
    document.getElementById('root')
);
