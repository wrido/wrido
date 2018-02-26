import {HubConnection} from '@aspnet/signalr';
import {Observable} from 'rxjs';
import {onInputChangeAction} from "../actionCreators";

const connection = new HubConnection('/query');

export const backendStreamEpic = () =>
  Observable
    .fromPromise(connection
      .start()
      .then(() => connection.stream('CreateResponseStream')))
    .flatMap(backendEvent =>
      // hack: SignalR observable is not same as RxJS :(
      Observable.create(observer => backendEvent.subscribe(observer))
    );

export const inputEpic = (action$) => {
  return action$
    .ofType(onInputChangeAction.type)
    .debounceTime(100)
    .map(inputAction => connection.invoke('QueryAsync', inputAction.value))
    .filter(() => false);
};