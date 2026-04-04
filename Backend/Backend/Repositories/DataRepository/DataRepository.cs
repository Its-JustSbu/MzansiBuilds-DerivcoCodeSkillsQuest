using Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Backend.Repositories.DataRepository
{
    public class DataRepository(AppDbContext context) : IDataRepository
    {
        async Task IDataRepository.AddAsync<T>(T entity)
        {
            await context.Set<T>().AddAsync(entity);
        }

        void IDataRepository.Delete<T>(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        async Task<IList<T>> IDataRepository.GetAsync<T>()
        {
            return await context.Set<T>().AsNoTracking().ToListAsync();
        }

        async Task<IList<T>> IDataRepository.GetByAsync<T>(Expression<Func<T, bool>> expression)
        {
            return await context.Set<T>().Where(expression).AsNoTracking().ToListAsync();
        }

        async Task IDataRepository.SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        void IDataRepository.Update<T>(T entity)
        {
            context.Update(entity);
        }
    }
}
