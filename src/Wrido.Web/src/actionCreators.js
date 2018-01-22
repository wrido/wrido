import { onInputChange, } from './constants';

export const onInputChangeAction = value => ({
  type: onInputChange,
  payload: {
    value
  }
});
