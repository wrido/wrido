import React from 'react';
import { connect } from 'react-redux';

const Result = ({ items }) => {
  return (
    <div>
      {
        items.map((item, i) => (
          <div key={i}>
            <img src={item.resources.$values[0].uri} alt={item.resources.$values[0].alt} />
            {item.title}
            <em>({item.description})</em>
          </div>
        ))
      }
    </div>
  );
};

export default connect(
  ({ result }) => result,
  {}
)(Result);
