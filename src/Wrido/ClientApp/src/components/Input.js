import React from 'react';
import {connect} from 'react-redux';
import {onInputChangeAction, selectNextResult, selectPreviousResult, clearQuery, hideShell} from '../actionCreators';

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

// TODO: Resolve from config?
const keyMap = {
  nextItem: 'ArrowDown',
  previousItem: 'ArrowUp',
  clearOrHide: 'Escape'
}

class Input extends React.Component {

  constructor() {
    super();
    this.handleKeyDown.bind(this);
  }

  componentDidMount() {
    // force focus on the input element
    this.domElement.focus();
    this.domElement.onblur = () => this.domElement.focus();
  }

  handleKeyDown = (event) => {
    if (event.key === keyMap.nextItem) {
      this.props.selectNextResult();
      event.preventDefault();
    }
    if (event.key === keyMap.previousItem) {
      this.props.selectPreviousResult();
      event.preventDefault();
    }
    if (event.key === keyMap.clearOrHide) {
      if(this.props.value){
        this.props.clearQuery();
      }
      else {
        this.props.hideShell();
      }
    }
  };

  render() {
    return (
      <div style={style.div}>
        <input
          ref={elem => this.domElement = elem}
          value={this.props.value}
          onChange={e => this.props.onInputChangeAction(e.target.value)}
          onKeyDown={this.handleKeyDown}
          style={style.input}/>
      </div>
    )
  }
}

export default connect(
  ({input}) => input,
  {onInputChangeAction, selectNextResult, selectPreviousResult, clearQuery, hideShell}
)(Input);
