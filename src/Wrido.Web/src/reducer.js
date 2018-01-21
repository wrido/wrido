import { combineReducers } from "redux";

const initialState = {
  value: 'success'
};

const inputReducer = (state = initialState, action) => {
  switch (action.type) {
    case ('onInputChange'):
      return {
        ...state,
        value: action.payload.value
      }
    default:
      return state;
  }
}

export const rootReducer = combineReducers({
  input: inputReducer
})
