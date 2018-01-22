import { onInputChange } from '../constants';

const initialInputState = {
  value: 'success'
};

export const input = (state = initialInputState, action) => {
  switch (action.type) {
    case onInputChange:
      return {
        ...state,
        value: action.payload.value
      }
    default:
      return state;
  }
}

const initialStatusState = 'not set';

export const status = (state = initialStatusState, action) => {
  switch (action.type) {
    case onInputChange:
      return 'unknown';
    default:
      return state;
  }
}