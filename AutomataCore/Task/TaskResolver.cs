using AutomataCore.Algorithm;
using Microsoft.Extensions.DependencyInjection;

namespace AutomataCore.Task;

public class TaskResolver
{
    private static readonly IServiceProvider serviceProvider;
    
    static TaskResolver()
    {
        var services = new ServiceCollection()
            .AddSingleton<ITask, DeterminizationTask>()
            .AddSingleton<ITask, MinimizationTask>();
        serviceProvider = services.BuildServiceProvider();
    }

    public static ITask GetService(string name)
    {
        if (name == DeterminizationAlgorithm.GetInstance().Name)
            return serviceProvider.GetRequiredService<DeterminizationTask>();
        if(name == MinimizationAlgorithm.GetInstance().Name)
            return serviceProvider.GetRequiredService<MinimizationTask>();
        throw new ArgumentException($"service {name} does not exist");
    }

    public static IEnumerable<ITask> GetAllServices()
    {
        return serviceProvider.GetServices<ITask>();
    }
}