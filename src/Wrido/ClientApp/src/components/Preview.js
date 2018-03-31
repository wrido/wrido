import React from 'react';
import { connect } from 'react-redux';

const style = {
  overflow: 'hidden',
  width: '50%'
};

const iframSrcFactory = (scriptSrc) => `
<html>
    <head>
        <script src='${scriptSrc}'></script>
        <script>
            window.onMessage = function(e) {
              console.log('data received', e);
              window.latest = e.data;
            }
        </script>
    </head>
    <body>
        Hello, world ${new Date()}
    </body>
</html>
`;

const Preview = ({active, previewEnabled}) => {
  if(!active || !previewEnabled){
    return '';
  }

  if(active.renderer){
    return <iframe style={style} sandbox="allow-scripts" srcDoc={iframSrcFactory(active.renderer.uri)} width="100%" height="450px" />
  }

  if(active.uri) {
    return (<iframe style={style} src={active.previewUri} width="100%" height="450px" />)
  }

  return null;
}

export default connect(
  ({result}) => result,
  {}
  )(Preview);