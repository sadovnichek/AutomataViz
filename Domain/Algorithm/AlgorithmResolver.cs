using Microsoft.Extensions.DependencyInjection;

namespace Domain.Algorithm;

public static class AlgorithmResolver
{
    private static readonly IServiceProvider serviceProvider;

    static AlgorithmResolver()
    {
        var services = new ServiceCollection()
                .AddSingleton<IAlgorithm, AcceptWordAlgorithm>()
                .AddSingleton<IAlgorithm, MinimizationAlgorithm>()
                .AddSingleton<IAlgorithm, DeterminizationAlgorithm>();
        
        serviceProvider = services.BuildServiceProvider();
    }

    public static T GetService<T>()
        where T : IAlgorithm
    {
        return (T) serviceProvider.GetServices<IAlgorithm>().First(a => a is T);
    }

    public static IAlgorithm GetServiceByName(string serviceName)
    {
        return GetAllServices().First(x => x.Name == serviceName);
    }

    public static IEnumerable<IAlgorithm> GetAllServices()
    {
        return serviceProvider.GetServices<IAlgorithm>();
    }
}