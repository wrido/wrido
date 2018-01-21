import React from 'react'
import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'
import { onInputChange } from '../actionCreators'

const Input = props => {
  const onChange = e => props.onInputChange(e.target.value);
  return (
    <div>
      <input value={props.value} onChange={onChange} />
    </div>
  )
}

const mapDispatchToProps = dispatch => bindActionCreators({
  onInputChange
}, dispatch)

export default connect(
  state => {
    console.log(state);
    return state.input;
  },
  mapDispatchToProps
)(Input)