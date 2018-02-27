import React from 'react';
import {connect} from 'react-redux';
import {onInputChangeAction, handleKeyPress} from '../actionCreators';

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

const preventDefaultKeys = ['ArrowDown', 'ArrowUp'];

class Input extends React.Component {

  componentDidMount() {
    // force focus on the input element
    this.domElement.focus();
    this.domElement.onblur = () => this.domElement.focus();
  }

  dispatchKeyDownAction = (event) => {
    this.props.handleKeyPress(event.key);
    if(preventDefaultKeys.some(k => event.key === k)){
      event.preventDefault();
    }
  }

  render() {
    return (
      <div style={style.div}>
        <input
          ref={elem => this.domElement = elem}
          value={this.props.value}
          onChange={e => this.props.onInputChangeAction(e.target.value)}
          onKeyDown={e => this.dispatchKeyDownAction(e)}
          style={style.input}/>
      </div>
    )
  }
}

export default connect(
  ({input}) => input,
  {onInputChangeAction, handleKeyPress}
)(Input);
