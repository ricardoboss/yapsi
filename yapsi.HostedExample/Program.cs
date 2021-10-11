using yapsi;
using yapsi.Default;
using yapsi.HostedExample;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
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
    .Build();

await host.RunAsync();
