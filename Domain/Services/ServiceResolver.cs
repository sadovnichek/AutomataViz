using Microsoft.Extensions.DependencyInjection;

namespace Domain.Services;

public class ServiceResolver : IServiceResolver
{
    private readonly IServiceProvider serviceProvider;
    public ServiceResolver()
    {
        serviceProvider = new ServiceCollection()
                .AddSingleton<IService, WordRecognitionAlgorithm>()
                .AddSingleton<IService, MinimizationAlgorithm>()
                .AddSingleton<IService, DeterminizationAlgorithm>()
                .AddSingleton<IService, VisualizationService>()
                .AddSingleton<IService, RandomAutomataService>()
                .BuildServiceProvider();
    }

    public T GetService<T>()
    {
        return (T)serviceProvider.GetServices<IService>().First(a => a is T);
    }

    public IAlgorithm GetAlgorithmByName(string serviceName)
    {
        return GetAllAlgorithms().First(x => x.Name == serviceName);
    }

    public IEnumerable<IAlgorithm> GetAllAlgorithms()
    {
        return serviceProvider.GetServices<IService>()
            .Where(service => service is IAlgorithm)
            .Select(service => (IAlgorithm)service);
    }
}