using System.Linq.Expressions;

namespace BookLending.Application.RepositoryContract
{
    public interface IGenericRepository<T> where T : class
    {
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        IQueryable<T> GetAllAsNoTracking();
        IQueryable<T> GetFiltered(Expression<Func<T, bool>> expression, bool asTracking = false);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
    }
}