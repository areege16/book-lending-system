using BookLending.Application.RepositoryContract;

namespace BookLending.Application.UnitOfWorkContract
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
