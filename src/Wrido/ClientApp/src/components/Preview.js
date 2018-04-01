import React from 'react';
import { connect } from 'react-redux';

const style = {
  overflow: 'hidden',
  width: '50%'
};

class PreviewClass extends React.Component {
  componentDidUpdate(){
    if(!this.iframe){
      return;
    }
    this.iframe.contentWindow.postMessage(this.props.active, '*');
  }

  render(){
    if(!this.props.active || !this.props.previewEnabled){
      return null;
    }

    if(this.props.active.previewUri){
      return <iframe
        style={style}
        title={this.props.active.id}
        sandbox="allow-scripts"
        src={this.props.active.previewUri}
        ref={elem => this.iframe = elem}
        width="100%" height="450px" />
    }

    return null;
  }
}

export default connect(
  ({result}) => result,
  {}
  )(PreviewClass);