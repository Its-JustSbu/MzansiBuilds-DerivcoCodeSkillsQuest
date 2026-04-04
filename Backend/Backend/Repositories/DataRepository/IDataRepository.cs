using System.Linq.Expressions;

namespace Backend.Repositories.DataRepository
{
    public interface IDataRepository
    {
        void Delete<T>(T entity) where T : class;
        void DeleteRange<T>(List<T> entity) where T : class;
        void Update<T>(T entity) where T : class;
        void UpdateRange<T>(List<T> entities) where T : class;
        Task AddangeAsync<T>(List<T> entities) where T: class;
        Task AddAsync<T>(T entity) where T: class;
        Task<IList<T>> GetAsync<T>() where T : class;
        Task<IList<T>> GetByAsync<T>(Expression<Func<T, bool>> expression) where T : class;
        Task SaveChangesAsync();
    }
}
