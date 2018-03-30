import React from 'react';
import { connect } from 'react-redux';

const style = {
  overflow: 'hidden',
  width: '50%'
};

const Preview = ({active, previewEnabled}) => {
  if(!active || !previewEnabled){
    return '';
  }

  return (
    <div style={style}>Preview for {active.title}</div>
  )
}

export default connect(
  ({result}) => result,
  {}
  )(Preview);