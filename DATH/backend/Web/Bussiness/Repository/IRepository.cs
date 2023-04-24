using Entities.Interface;

namespace Bussiness.Repository
{
    public interface IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity?> GetAsync(TPrimaryKey id, CancellationToken cancellationToken = default);
        Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TPrimaryKey id, CancellationToken cancellationToken = default);
    }

    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class, IEntity<int>
    {
    }
}
