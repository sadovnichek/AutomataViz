using Microsoft.Extensions.DependencyInjection;

namespace Application.StudentTask;

public static class TaskResolver
{
    private static readonly IServiceProvider serviceProvider;
    
    static TaskResolver()
    {
        var services = new ServiceCollection()
            .AddSingleton<IAutomataTask, DeterminizationTask>()
            .AddSingleton<IAutomataTask, MinimizationTask>();
        serviceProvider = services.BuildServiceProvider();
    }

    public static T GetService<T>()
        where T : IAutomataTask
    {
        return (T) serviceProvider.GetServices<IAutomataTask>().First(a => a is T);
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