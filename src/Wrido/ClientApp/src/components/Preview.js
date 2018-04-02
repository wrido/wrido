import React from 'react';
import { connect } from 'react-redux';

const style = {
  overflow: 'hidden',
  width: '50%',
  height: '93vh',
  margin: '0px',
  borderTop: '1px solid #ccc'
};

class PreviewClass extends React.Component {
  componentDidUpdate(){
    if(!this.iframe){
      return;
    }
    else {
      this.iframe.contentWindow.postMessage(this.props.active, '*');
      this.iframe.onload = () => this.iframe.contentWindow.postMessage(this.props.active, '*');
    }
  }

  render(){
    if(!this.props.active || !this.props.previewEnabled){
      return null;
    }
    console.log('render progress', this.props.active.playbackProgress);

    if(this.props.active.previewUri){
      return <iframe
        style={style}
        title={this.props.active.id}
        sandbox="allow-scripts"
        src={this.props.active.previewUri}
        ref={elem => this.iframe = elem} />
    }

    return null;
  }
}

export default connect(
  ({result}) => result,
  {}
  )(PreviewClass);