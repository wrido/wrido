import React from 'react';
import { connect } from 'react-redux';
import { onInputChangeAction } from '../actionCreators';

const style = {
  input: {
    width: '100%',
    fontSize: '25px',
    backgroundColor: '#eee',
    outline: 'none',
    border: 'none',
    },
  div: {
      backgroundColor: '#eee',
      padding: '5px 15px'
  }
}

const Input = ({ onInputChangeAction, value }) => (
  <div style={style.div}>
    <input value={value} onChange={e => onInputChangeAction(e.target.value)} style={style.input} />
  </div>
);

export default connect(
  ({ input }) => input,
  { onInputChangeAction }
)(Input);
