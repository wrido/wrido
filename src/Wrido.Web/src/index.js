import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';
import { connect } from './connect';

connect();
ReactDOM.render(<App />, document.getElementById('root'));
