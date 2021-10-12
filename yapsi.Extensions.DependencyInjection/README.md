# yapsi.Extensions.DependencyInjection

Dependency injection extensions to ease the development with yapsi.

## Usage:

This simple extension method registers a `Pipeline` as a singleton in your service provider
and adds the appropriate factories for `ISubscription` and `IContract`:

```csharp
protected override void ConfigureServices(HostBuilderContext context, IServiceCollection collection)
{
    collection.AddYapsi<string>();

    collection.AddYapsi<CustomType>();
}
```

...becomes...

```csharp
protected override void ConfigureServices(HostBuilderContext context, IServiceCollection collection)
{
    collection.AddSingleton<Pipeline<string>>()
              .AddTransient(sp => sp.GetRequiredService<Pipeline<string>>().Subscribe())
              .AddTransient(sp => sp.GetRequiredService<Pipeline<string>>().Bind());

    collection.AddSingleton<Pipeline<CustomType>>()
              .AddTransient(sp => sp.GetRequiredService<Pipeline<CustomType>>().Subscribe())
              .AddTransient(sp => sp.GetRequiredService<Pipeline<CustomType>>().Bind());
}
```

...so you can simply inject `ISubscription<CustomType>` into your services:

```csharp
public class MyService {
    private readonly ISubscription<CustomType> _customTypeSubscription;

    public MyService(ISubscription<CustomType> customTypeSubscription) {
        _customTypeSubscription = customTypeSubscription;
        _customTypeSubscription.Published += HandleCustomTypePublished;
    }

    private void HandleCustomTypePublished(ISubscription<CustomType> sender, CustomType data) {
        // do something with your data
    }
}
```
