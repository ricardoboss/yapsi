namespace yapsi.HostedExample
{
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
}
