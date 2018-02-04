let connection = new signalR.HubConnection('/query');

// Confirmation that query has been recieved and is about to get executed.
// Message contains a unique query id that will be present for other event
// related to query.
const queryReceived = 'QueryReceived';

// Triggered as the applicable query providers start executing the query.
// The message contains a list of query providers that will execute the query.
// This list can be used to setup toggle load state etc.
const queryExecuting = 'QueryExecuting';

// DEPRECATED, use 'resultAvailable' instead
// Triggered each time a query provider has completed its execution.
// This event marks a partial completion of the query
const resultsAvailable = 'ResultsAvailable';

// Triggered each time a query provider has completed its execution.
// This event marks a partial completion of the query
const resultAvailable = 'ResultAvailable';

// Triggered by provider to idicated that existing result is updated.
// Can be called multiple time for each result
const resultUpdated = 'ResultUpdated';

// Triggered when all applicable Query Providers have produced result.
const queryCompleted = 'QueryCompleted';

// Indicates that the query has been cancelled and no more results will
// be produced.
const queryCancelled = 'QueryCancelled';

connection.start().then(() => {
    let inputElement = document.getElementById('wrido-text');
    let inputStreamElement = document.getElementById('wrido-text-streamed');
    let inputLegacyElement = document.getElementById('wrido-text-legacy');
    let resultElement = document.getElementById('wrido-result');
    let queryIdElement = document.getElementById('wrido-queryid');
    let statusElement = document.getElementById('wrido-status');

    let onQueryReceived = msg => {
        queryIdElement.innerHTML = msg.current.id;
        statusElement.innerHTML = 'ongoing';
        resultElement.innerHTML = '';
    }

    let onResultAvailable = msg => {
        statusElement.innerHTML = 'partial complete';
        let li = document.createElement('li');
        let result = msg.result;

        if (result.icon) {
            let img = document.createElement('img');
            img.alt = result.icon.alt;
            img.src = result.icon.uri;
            li.appendChild(img);
        }
        var span = document.createElement('span');
        span.innerHTML = `${result.title} <em>(${result.description})</em>`;
        li.appendChild(span);
        span.onclick = () => connection.invoke('ExecuteAsync', result);
        resultElement.appendChild(li);
    };

    let onResultsAvailable = msg => {
        for (let i = 0; i < msg.results.$values.length; i++) {
            onResultAvailable({ result: msg.results.$values[i] });
        }
    }

    let onQueryComplete = msg => {
        statusElement.innerHTML = 'completed';
    };

    let dispatchEvent = msg => {
        switch (msg.type) {
            case queryReceived:
                onQueryReceived(msg);
                break;
            case queryExecuting:
                console.debug('query executing', msg);
                break;
            case resultAvailable:
                onResultAvailable(msg);
                break;
            case resultsAvailable:
                onResultsAvailable(msg);
                break;
            case queryCompleted:
                onQueryComplete(msg);
                break;
            case resultUpdated:
                console.debug('result updated', msg);
            default:
                console.log('message not handled', msg);
        }
    }

    inputStreamElement.oninput = ev => {
        statusElement.innerHTML = 'unknown';
        connection.stream('StreamQueryEvents', ev.target.value).subscribe({
            close: false,
            next: msg => dispatchEvent(msg),
            error: function (err) {
                console.log(err);
            },
            complete: onQueryComplete
        });
    };

    inputElement.oninput = ev => {
        statusElement.innerHTML = 'unknown';
        connection.invoke('QueryAsync', ev.target.value);
    };

    connection.on('event', msg => dispatchEvent(msg));
});