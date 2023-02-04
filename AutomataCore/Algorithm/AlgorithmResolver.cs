using Microsoft.Extensions.DependencyInjection;

namespace AutomataCore.Algorithm;

public static class AlgorithmResolver
{
    private static readonly IServiceProvider serviceProvider;

    static AlgorithmResolver()
    {
        var services = new ServiceCollection()
            .AddSingleton<IAlgorithm, AcceptWordAlgorithm>()
            .AddSingleton<IAlgorithm, MinimizationAlgorithm>()
            .AddSingleton<IAlgorithm, DeterminizationAlgorithm>()
            .AddSingleton<AcceptWordAlgorithm>()
            .AddSingleton<MinimizationAlgorithm>()
            .AddSingleton<DeterminizationAlgorithm>();
        
        serviceProvider = services.BuildServiceProvider();
    }

    public static T GetService<T>()
        where T : IAlgorithm
    {
        return (T) serviceProvider.GetRequiredService(typeof(T));
    }

    public static IEnumerable<IAlgorithm> GetAllServices()
    {
        return serviceProvider.GetServices<IAlgorithm>();
    }
}