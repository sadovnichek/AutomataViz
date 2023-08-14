namespace Domain.Services
{
    public interface IServiceResolver
    {
        T GetService<T>();

        IAlgorithm GetAlgorithmByName(string name);

        IEnumerable<IAlgorithm> GetAllAlgorithms();
    }
}