namespace API.Interface
{
    public interface IRepository<TEntity> where TEntity : class, IEntity<long>
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity?> GetAsync(long id);
        Task<long> InsertAndGetIdAsync(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(long id);
    }
}
