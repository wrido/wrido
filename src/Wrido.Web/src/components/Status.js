import React from 'react'
import { connect } from 'react-redux'

const Status = props => {
  return (
    <p>
      <strong>{props.status}</strong>
    </p>
  )
}

export default connect(({ status }) => ({ status }))(Status);