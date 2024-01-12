namespace ClientApp.Data.Repositories
{
    public interface IReadOnlyRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<T?> GetLatestInLocation(string location);
        Task<IEnumerable<T>> GetAllByLocation(string location);
        Task<IEnumerable<T>> GetLatestInEveryLocation();
    }
}