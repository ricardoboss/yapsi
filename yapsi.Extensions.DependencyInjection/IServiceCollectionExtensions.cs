using Microsoft.Extensions.DependencyInjection;

using yapsi.Default;

namespace yapsi.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddYapsi<T>(this IServiceCollection serviceProvider)
        {
            return serviceProvider
                .AddSingleton<Pipeline<T>>()
                .AddTransient(sp => sp.GetRequiredService<Pipeline<T>>().Subscribe())
                .AddTransient(sp => sp.GetRequiredService<Pipeline<T>>().Bind());
        }
    }
}
