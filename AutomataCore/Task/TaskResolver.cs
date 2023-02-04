using AutomataCore.Algorithm;
using Microsoft.Extensions.DependencyInjection;

namespace AutomataCore.Task;

public static class TaskResolver
{
    private static readonly IServiceProvider serviceProvider;
    
    static TaskResolver()
    {
        var services = new ServiceCollection()
            .AddSingleton<IAutomataTask, DeterminizationAutomataTask>()
            .AddSingleton<IAutomataTask, MinimizationAutomataTask>();
        serviceProvider = services.BuildServiceProvider();
    }

    public static T GetService<T>()
        where T : IAutomataTask
    {
        return (T)serviceProvider.GetRequiredService(typeof(T));
    }

    public static IAutomataTask GetServiceByName(string serviceName)
    {
        return GetAllServices().First(service => service.Name == serviceName);
    }
    
    public static IEnumerable<IAutomataTask> GetAllServices()
    {
        return serviceProvider.GetServices<IAutomataTask>();
    }
}