using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Database;
using Entities.Interface;
using Microsoft.AspNetCore.Http;

namespace Bussiness.Repository
{
    public class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        private readonly DataContext _dbContext;
        private readonly ISession _session;

        public Repository(
            DataContext dbContext,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _dbContext = dbContext;
            _session = httpContextAccessor.HttpContext!.Session;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>();
        }

        public async Task<TEntity?> GetAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            TEntity? entity = await _dbContext.Set<TEntity>().FindAsync(new object?[] { id }, cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasCreatorUserId creatorUserId)
            {
                creatorUserId.CreatorUserId = _session.GetString("UserId") == null ? null : long.Parse(_session.GetString("UserId")!);
            }
            await _dbContext.Set<TEntity>().AddAsync(entry.Entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasLastModifierUserId lastModifierUserId)
            {
                lastModifierUserId.LastModifierUserId = _session.GetString("UserId") == null ? null : long.Parse(_session.GetString("UserId")!);
                if (entry.State == EntityState.Detached)
                {
                    entry.State = EntityState.Modified;
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entry.Entity;
        }

        public async Task DeleteAsync(TPrimaryKey id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return;
            }

            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is ISoftDelete deleteUserId)
            {
                deleteUserId.DeleteUserId = _session.GetString("UserId") == null ? null : long.Parse(_session.GetString("UserId")!);
            }

            _dbContext.Set<TEntity>().Remove(entry.Entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasCreatorUserId creatorUserId)
            {
                creatorUserId.CreatorUserId = _session.GetString("UserId") == null ? null : long.Parse(_session.GetString("UserId")!);
            }
            TEntity e = (await _dbContext.Set<TEntity>().AddAsync(entry.Entity, cancellationToken)).Entity;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return e.Id;
        }
    }

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity<int>
    {
        private readonly DataContext _dbContext;
        private readonly ISession _session;

        public Repository(
            DataContext dbContext,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _dbContext = dbContext;
            _session = httpContextAccessor.HttpContext!.Session;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>();
        }

        public async Task<TEntity?> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            TEntity? entity = await _dbContext.Set<TEntity>().AsNoTracking().Where(e => e.Id  == id).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            return entity;
        }

        public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasCreatorUserId creatorUserId)
            {
                creatorUserId.CreatorUserId = _session.GetString("UserId") == null ? null : long.Parse(_session.GetString("UserId")!);
            }
            await _dbContext.Set<TEntity>().AddAsync(entry.Entity, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasLastModifierUserId lastModifierUserId)
            {
                lastModifierUserId.LastModifierUserId = _session.GetString("UserId") == null ? null : long.Parse(_session.GetString("UserId")!);
                if (entry.State == EntityState.Detached)
                {
                    entry.State = EntityState.Modified;
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entry.Entity;
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return;
            }

            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is ISoftDelete deleteUserId)
            {
                deleteUserId.DeleteUserId = _session.GetString("UserId") == null ? null : long.Parse(_session.GetString("UserId")!);
            }

            _dbContext.Set<TEntity>().Remove(entry.Entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<int> InsertAndGetIdAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasCreatorUserId creatorUserId)
            {
                creatorUserId.CreatorUserId = _session.GetString("UserId") == null ? null : long.Parse(_session.GetString("UserId")!);
            }
            TEntity e = (await _dbContext.Set<TEntity>().AddAsync(entry.Entity)).Entity;
            await _dbContext.SaveChangesAsync(cancellationToken);
            return e.Id;
        }
    }
}