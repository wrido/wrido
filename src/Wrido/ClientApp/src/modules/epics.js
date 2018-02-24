import {handleKeyPress} from "../actionCreators";

export searchTermEpic = action$ => {
  action$
    .ofType(handleKeyPress.type)
    .map(action => action.event.foo)
}