import React from 'react';
import { connect } from 'react-redux';

const Result = ({ items }) => {
  return (
    <div>
      {
        items.map((item, i) => (
          <div key={i}>{item.title}</div>
        ))
      }
    </div>
  );
};

export default connect(
  ({ result }) => result,
  {}
)(Result);
