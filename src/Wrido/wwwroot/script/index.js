let connection = new signalR.HubConnection('/input');

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
    let inputElement = document.getElementById('wrido-text');
    let resultElement = document.getElementById('wrido-result');
    let queryIdElement = document.getElementById('wrido-queryid');
    let statusElement = document.getElementById('wrido-status');
    
    inputElement.oninput = ev => {
        statusElement.innerHTML = 'unknown';
        connection.invoke('QueryAsync', ev.target.value);
    };

    connection.on(queryReceived, msg => {
        console.log(queryReceived, msg);
        queryIdElement.innerHTML = msg.current.id;
        statusElement.innerHTML = 'ongoing';
        resultElement.innerHTML = '';
    });

    connection.on(resultsAvailable, msg => {
        statusElement.innerHTML = 'partial complete';
        for (let i = 0; i < msg.results.length; i++) {
            let li = document.createElement('li');
            li.innerHTML = msg.results[i].title + ` <em>(${msg.results[i].description})</em>`;
            resultElement.appendChild(li);
        }
    });

    connection.on(queryCompleted, msg => {
        statusElement.innerHTML = 'completed';
    });
});