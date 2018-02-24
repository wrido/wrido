import React from 'react';
import 'reset-css';
import Input from './components/Input';
import Result from './components/Result';

export default props => (
  <div>
    <style>
      {`
        .listItem{background-color: #eee}
      `}
    </style>
    <Input />
    <Result />
  </div>
)
