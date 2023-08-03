using Microsoft.Extensions.DependencyInjection;

namespace Domain.Algorithm;

public static class AlgorithmResolver
{
    private static readonly IServiceProvider serviceProvider;
    static AlgorithmResolver()
    {
        serviceProvider = new ServiceCollection()
                .AddSingleton<IService, WordRecognitionAlgorithm>()
                .AddSingleton<IService, MinimizationAlgorithm>()
                .AddSingleton<IService, DeterminizationAlgorithm>()
                .AddSingleton<IService, VisualizationService>()
                .BuildServiceProvider();
    }

    public static T GetService<T>()
        where T : class
    {
        return (T)serviceProvider.GetServices<IService>().First(a => a is T);
    }

    public static IAlgorithm GetAlgorithmByName(string serviceName)
    {
        return GetAllAlgorithms().First(x => x.Name == serviceName);
    }

    public static IEnumerable<IAlgorithm> GetAllAlgorithms()
    {
        return serviceProvider.GetServices<IService>()
            .Where(service => service is IAlgorithm)
            .Select(service => (IAlgorithm)service);
    }
}