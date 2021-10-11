# yapsi

> yet another pub-sub implementation

Provides simple interfaces for publishing/subscribing to generic data "packets".

## Usage

```csharp
using yapsi.Default;

// create a pipeline somewhere and make it available (through dependency injection for example)
// the generic parameter is the type of data being transported by the pipeline
var pipeline = new Pipeline<string>();

// ...somewhere you want to provide data from
var contract = pipeline.Bind();

// ...somewhere else where you want to consume the data
var subscription = pipeline.Subscribe();
subscription.Published += (sender, message) => Console.WriteLine(message);

// publish some data
contract.Publish("Hello from the other side");
contract.Publish("Hello again");
```

Run the `yapsi.Example` project to see how it works.

You can add as many contracts and subscriptions you want, which means you could create a factory for injecting subscriptions into your services:

`Program.cs`
```csharp
    // ...
    
    .ConfigureServices(services =>
    {
        // ...
    
        services.AddSingleton<Pipeline<string>>();

        services.AddTransient<IContract<string>>((sp) =>
        {
            var pipeline = sp.GetRequiredService<Pipeline<string>>();

            return pipeline.Bind();
        });

        services.AddTransient<ISubscription<string>>((sp) =>
        {
            var pipeline = sp.GetRequiredService<Pipeline<string>>();

            return pipeline.Subscribe();
        });

        services.AddHostedService<Worker>();
    })
    
    // ...
```

`Worker.cs`
```csharp
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISubscription<string> _subscription;
        private readonly IContract<string> _contract;

        public Worker(ILogger<Worker> logger, ISubscription<string> subscription, IContract<string> contract)
        {
            _logger = logger;
            _subscription = subscription;
            _contract = contract;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // ideally, you would have some service supplying data and another service consuming the data, but
            // this example shows how the dependency injection works
            
            _subscription.Published += (sender, data) =>
            {
                _logger.LogInformation("Received data from pipeline: {data}", data);
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                _contract.Publish($"Worker running at: {DateTimeOffset.Now}");

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
```

Run the `yapsi.HostedExample` project to see this in action.
