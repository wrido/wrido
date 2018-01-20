let logger = new signalR.HubConnection('/logging');

// Available Log levels
const verbose = 'Verbose';
const debug = 'Debug';
const information = 'Information';
const warning = 'Warning';
const error = 'Error';
const critical = 'Critical';

// Begins a logging scope with name and value matching
// provided arguments. A logging scope will enrich
// log entries with the property.
const beginScope = 'BeginScope';

// Ends the latest scope and thereby removes the
// latest added property.
const endScope = 'EndScope';

// Performs a write operation.
// Arguments: logLevel, messageTemplate, propertyValues
const write = 'Write';

logger.start().then(() => {
    logger.invoke(beginScope, 'scopeName', 'scopeValue').then(() => {
        logger.invoke(write, information, "front end started at {renderTime}", [new Date()]);
        logger.invoke(write, information, "it turns out that {first} plus {second} is {sum}", [1, 2, 1+2]);
        logger.invoke(endScope);
    });
});