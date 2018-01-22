import React from 'react'
import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'
import { onInputChangeAction } from '../actionCreators'

const Input = ({ onInputChangeAction, input }) => {
  const onChange = e => onInputChangeAction(e.target.value);
  return (
    <div>
      <input value={input.value} onChange={onChange} />
    </div>
  )
}

const mapDispatchToProps = dispatch => bindActionCreators({
  onInputChangeAction
}, dispatch)

export default connect(
  ({ input }) => ({ input }),
  mapDispatchToProps
)(Input)