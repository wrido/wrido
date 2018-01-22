import React from 'react';
import { Provider } from 'react-redux';
import { render } from 'react-dom';
import App from './App';
import { connect } from './connect';
import store from './store';

connect();
render(
    <Provider store={store}>
        <App />
    </Provider>,
    document.getElementById('root')
);
