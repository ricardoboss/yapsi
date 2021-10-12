using Microsoft.Extensions.DependencyInjection;

using yapsi.Default;

namespace yapsi.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddYapsi<P, T>(this IServiceCollection serviceProvider) where P : Pipeline<T>
        {
            return serviceProvider
                .AddSingleton<P>()
                .AddTransient(sp => sp.GetRequiredService<P>().Subscribe())
                .AddTransient(sp => sp.GetRequiredService<P>().Bind());
        }
    }
}
