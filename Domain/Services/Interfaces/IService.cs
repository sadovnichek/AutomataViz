using Domain.Automatas;

namespace Domain.Services
{
    public interface IService
    {

    }

    public interface IVisualizationService : IService
    {
        void SaveAutomataImage(Automata automata, string folderToSave);
    }
}