using Domain.Automatas;
using System.Windows.Controls;

namespace AutomataUI.Workspaces
{
    public interface IWorkspace
    {
        void Init(StackPanel parent);
    }

    public interface IAutomataWorkspace : IWorkspace
    {
        void AddContent(Automata automata);
    }

    public interface IWordInputWorkspace : IWorkspace
    {
        void AddContent(string content);

        string GetInput();
    }
}