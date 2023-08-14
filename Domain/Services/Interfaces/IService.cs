using Domain.Automatas;

namespace Domain.Services
{
    public interface IService
    {

    }

    public interface IVisualizationService : IService
    {
        Uri GetImageUri(Automata automata);

        string GetBase64Image(Automata automata);
    }
}