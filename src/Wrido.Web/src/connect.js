export const connect = () => {
  let connection = new window.signalR.HubConnection('/query');

  // Confirmation that query has been recieved and is about to get executed.
  // Message contains a unique query id that will be present for other event
  // related to query.
  const queryReceived = 'QueryReceived';

  // Triggered as the applicable query providers start executing the query.
  // The message contains a list of query providers that will execute the query.
  // This list can be used to setup toggle load state etc.
  const queryExecuting = 'QueryExecuting';

  // Triggered each time a query provider has completed its execution.
  // This event marks a partial completion of the query
  const resultsAvailable = 'ResultsAvailable';

  // Triggered when all applicable Query Providers have produced result.
  const queryCompleted = 'QueryCompleted';

  // Indicates that the query has been cancelled and no more results will
  // be produced.
  const queryCancelled = 'QueryCancelled';

  connection.start().then(() => {
    connection.on(queryReceived, msg => {
      console.log(queryReceived, msg);
    });

    connection.on(resultsAvailable, msg => {
      console.log(resultsAvailable, msg);
    });

    connection.on(queryCompleted, msg => {
      console.log(queryCompleted, msg);
    });

    connection.invoke('QueryAsync', 'google test');
  });
}