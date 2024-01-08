namespace ClientApp.Data
{
    public interface IReadOnlyRepository<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<T?> GetLatest();
        Task<IEnumerable<T>> GetAllByLocation(string location);
        Task<IEnumerable<T>> GetLatestInEveryLocation();
    }
}