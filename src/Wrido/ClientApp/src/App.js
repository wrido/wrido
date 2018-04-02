import React from 'react';
import 'reset-css';
import Input from './components/Input';
import Result from './components/Result';
import Preview from './components/Preview';

const flexCss = {
  container : {
    display: 'flex',
  },
  result: {
    flex: '1'
  }
};

export default props => {
  return (
  <div>
    <style>
      {`
        .listItem{background-color: #eee}
      `}
    </style>
    <div><Input /></div>
    <div style={flexCss.container}>
      <div style={flexCss.result}><Result /></div>
      <Preview />
    </div>

  </div>
)}
