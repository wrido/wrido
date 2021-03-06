import React from 'react';
import { connect } from 'react-redux';

const style = {
  list: {
    display: 'flex',
    flexDirection: 'column',
    borderTop: '1px solid #ccc'
  },
  item: {
    display: 'flex',
    alignItems: 'center',
    borderBottom: '1px solid #ccc',
    padding: '5px 15px',
    overflow: 'hidden'
  },
  title: {
    paddingLeft: '10px',
  },
  description: {
    paddingLeft: '10px',
    fontStyle: 'italic',
  }
}

const Result = ({ items, active, previewEnabled }) => {
  return (
    <div style={style.list}>
      {
        items.map((item, i) => (
          <div key={i} style={{...style.item, 'backgroundColor' : (active && item.id === active.id) ? '#ddd' : null}} className={'listItem'}>
            <span>
              <img src={item.icon.uri} alt={item.icon.alt} />
            </span>
            <span style={style.title}>{item.title}</span>
            <span style={style.description}>{!previewEnabled ? item.description : null}</span>
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
