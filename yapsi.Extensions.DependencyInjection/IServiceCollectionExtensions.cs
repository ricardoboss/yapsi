using Microsoft.Extensions.DependencyInjection;

using yapsi.Default;

namespace yapsi.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddYapsi<T>(this IServiceCollection serviceProvider)
        {
            return serviceProvider
                .AddSingleton<IPipeline<T>, Pipeline<T>>()
                .AddTransient(sp => sp.GetRequiredService<IPipeline<T>>().Subscribe())
                .AddTransient(sp => sp.GetRequiredService<IPipeline<T>>().Bind());
        }

        public static IServiceCollection AddSingleSubscribeYapsi<T>(this IServiceCollection serviceProvider)
        {
            return serviceProvider
                .AddSingleton<ISingleSubscribePipeline<T>, SingleSubscribePipeline<T>>()
                .AddTransient(sp => sp.GetRequiredService<ISingleSubscribePipeline<T>>().Subscribe())
                .AddTransient(sp => sp.GetRequiredService<ISingleSubscribePipeline<T>>().Bind());
        }

        public static IServiceCollection AddSingleBindYapsi<T>(this IServiceCollection serviceProvider)
        {
            return serviceProvider
                .AddSingleton<ISingleBindPipeline<T>, SingleBindPipeline<T>>()
                .AddTransient(sp => sp.GetRequiredService<ISingleBindPipeline<T>>().Subscribe())
                .AddTransient(sp => sp.GetRequiredService<ISingleBindPipeline<T>>().Bind());
        }
    }
}
