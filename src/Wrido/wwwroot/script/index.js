let connection = new signalR.HubConnection('/input');

connection.start().then(() => {
    let inputElement = document.getElementById('wrido-text');
    let resultElement = document.getElementById('wrido-result');

    inputElement.oninput = ev => {
        connection.invoke('QueryAsync', ev.target.value);
    };

    connection.on('ResultAvailable', (results) => {
        resultElement.innerHTML = '';

        for (let i = 0; i < results.length; i++) {
            let li = document.createElement('li');
            li.innerHTML = results[i];
            resultElement.appendChild(li);
        }
    });
});