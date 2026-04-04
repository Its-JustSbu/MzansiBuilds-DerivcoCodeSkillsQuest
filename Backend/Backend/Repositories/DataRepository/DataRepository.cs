using Backend.Models;
using System.Linq.Expressions;

namespace Backend.Repositories.DataRepository
{
    public class DataRepository(AppDbContext context) : IDataRepository
    {
        Task IDataRepository.AddAsync<T>(T entity)
        {
            throw new NotImplementedException();
        }

        void IDataRepository.Delete<T>(T entity)
        {
            throw new NotImplementedException();
        }

        Task IDataRepository.GetAsync<T>()
        {
            throw new NotImplementedException();
        }

        Task IDataRepository.GetByAsync<T>(Expression<Func<T, bool>> expression)
        {
            throw new NotImplementedException();
        }

        Task IDataRepository.SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        void IDataRepository.Update<T>(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
