namespace AutomataUI.Workspaces
{
    public interface IWorkspaceResolver
    {
        T GetWorkspace<T>();
    }
}