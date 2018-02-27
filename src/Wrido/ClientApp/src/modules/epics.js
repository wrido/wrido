import * as actions from '../actionCreators';

// TODO: Resolve from config?
const keyMap = {
  nextItem: 'ArrowDown',
  previousItem: 'ArrowUp',
  clearOrHide: 'Escape',
  executeResult: 'Enter'
}

export const keyPressEpic = (action$, store) => action$
  .ofType(actions.handleKeyPress.type)
  .map(keyPress => {

    let state = store.getState();

    switch (keyPress.key) {
      case keyMap.nextItem:
        return actions.selectNextResult();
      case keyMap.previousItem:
        return actions.selectPreviousResult();
      case keyMap.clearOrHide:
        return state.input.value
          ? actions.clearQuery()
          : actions.hideShell();
      case keyMap.executeResult:
        if (state.result.active) {
          return actions.executeResult(state.result.active);
        }
    }
  })
  .filter(action => action);