using yapsi.Default;

// create a pipeline somewhere and make it available (through dependency injection for example)
var pipeline = new Pipeline<string>();

// ...somewhere you want to provide data from
var contract = pipeline.Bind();

// ...somewhere else where you want to consume the data
var subscription = pipeline.Subscribe();
subscription.Published += (sender, message) => Console.WriteLine(message);

// publish some data
contract.Publish("Hello from the other side");
contract.Publish("Hello again");
