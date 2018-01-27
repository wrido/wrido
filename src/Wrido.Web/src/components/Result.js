import React from 'react'
import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'

const Result = ({ onInputChangeAction, input }) => {
  return (
    <div>
      Success
    </div>
  )
}

const mapDispatchToProps = dispatch => bindActionCreators({
}, dispatch)

export default connect(
  ({ result }) => ({ result }),
  mapDispatchToProps
)(Result)