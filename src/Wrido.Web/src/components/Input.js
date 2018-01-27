import React from 'react';
import { connect } from 'react-redux';
import { onInputChangeAction } from '../actionCreators';

const Input = ({ onInputChangeAction, value }) => {
  const onChange = e => onInputChangeAction(e.target.value);
  return (
    <div>
      <input value={value} onChange={onChange} />
    </div>
  );
};

export default connect(
  ({ input }) => input,
  { onInputChangeAction }
)(Input);
