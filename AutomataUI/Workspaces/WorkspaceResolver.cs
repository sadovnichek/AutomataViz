using Microsoft.Extensions.DependencyInjection;
using System;

namespace AutomataUI.Workspaces
{
    public class WorkspaceResolver : IWorkspaceResolver
    {
        private readonly IServiceProvider serviceProvider;

        public WorkspaceResolver()
        {
            serviceProvider = new ServiceCollection()
                .AddSingleton<IAutomataWorkspace, AutomataWorkspace>()
                .AddSingleton<IWordInputWorkspace, WordInputWorkspace>()
                .BuildServiceProvider();
        }

        public T GetWorkspace<T>() => serviceProvider.GetService<T>();
    }
}