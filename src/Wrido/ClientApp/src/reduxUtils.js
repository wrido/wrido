export const createActionCreator = (type, payloader) => Object.assign(payload => ({ type, ...payloader(payload) }), { type });

export const reducer = (initialState, ...configs) => (state = initialState, action) =>
  configs
    .filter(config => isSameActionType(config[0], action))
    .reduce((state, config) => ({
      ...state,
      ...config[1](state, action)
    }), state);

export const isSameActionType = (a1, a2) => a1.type === a2.type;
