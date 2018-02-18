import React from 'react';
import { connect } from 'react-redux';
import { onInputChangeAction } from '../actionCreators';

const style = {
  input: {
    width: '100%',
    fontSize: '25px',
    padding: '5px 15px',
    backgroundColor: '#eee',
    outline: 'none',
    border: 'none',
  }
}

const Input = ({ onInputChangeAction, value }) => (
  <div>
    <input value={value} onChange={e => onInputChangeAction(e.target.value)} style={style.input} />
  </div>
);

export default connect(
  ({ input }) => input,
  { onInputChangeAction }
)(Input);
