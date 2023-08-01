using Domain.Algorithm;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomataUI.Workspaces
{
    public class WorkspaceResolver
    {
        private static readonly IServiceProvider serviceProvider;

        static WorkspaceResolver()
        {
            serviceProvider = new ServiceCollection()
                .AddSingleton<IAutomataWorkspace, AutomataWorkspace>()
                .AddSingleton<IWordInputWorkspace, WordInputWorkspace>()
                .BuildServiceProvider();
        }

        public static T GetService<T>()
            where T : IWorkspace => serviceProvider.GetService<T>();
    }
}