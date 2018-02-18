import { onInputChange } from '../constants';

export const inputEpic = (action$) => action$
  .filter(a => a.type === onInputChange)
  .map();