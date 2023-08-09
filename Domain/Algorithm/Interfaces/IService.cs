using Domain.Automatas;

namespace Domain.Algorithm
{
    public interface IService
    {

    }

    public interface IVisualizationService : IService
    {
        Uri GetImageUri(Automata automata);
    }
}
