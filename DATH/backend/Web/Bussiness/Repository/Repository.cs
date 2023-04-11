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

        public Repository(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>();
        }

        public async Task<TEntity?> GetAsync(TPrimaryKey id)
        {
            TEntity? entity = await _dbContext.Set<TEntity>().FindAsync(id);
            return entity;
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasCreatorUserId creatorUserId)
            {
                //creatorUserId.CreatorUserId = User.GetUserId();
            }
            await _dbContext.Set<TEntity>().AddAsync(entry.Entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasLastModifierUserId lastModifierUserId)
            {
                //lastModifierUserId.LastModifierUserId = User.GetUserId();
                if (entry.State == EntityState.Detached)
                {
                    _dbContext.Update(entry.Entity);
                }
                else
                {
                    entry.State = EntityState.Modified;
                }
            }
            entry.State = EntityState.Modified;
            _dbContext.Update(entry.Entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task DeleteAsync(TPrimaryKey id)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return;
            }

            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is ISoftDelete deleteUserId && entry.State == EntityState.Deleted)
            {
                //deleteUserId.DeleteUserId = User.GetUserId();
            }

            _dbContext.Set<TEntity>().Remove(entry.Entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasCreatorUserId creatorUserId)
            {
                //creatorUserId.CreatorUserId = User.GetUserId();
            }
            TEntity e = (await _dbContext.Set<TEntity>().AddAsync(entry.Entity)).Entity;
            await _dbContext.SaveChangesAsync();
            return e.Id;
        }
    }

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity<int>
    {
        private readonly DataContext _dbContext;
        private readonly object? _session;

        public Repository(
            DataContext dbContext,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _dbContext = dbContext;
            _session = httpContextAccessor.HttpContext!.Items.FirstOrDefault().Value;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _dbContext.Set<TEntity>();
        }

        public async Task<TEntity?> GetAsync(int id)
        {
            TEntity? entity = await _dbContext.Set<TEntity>().AsNoTracking().Where(e => e.Id  == id).FirstOrDefaultAsync();
            return entity;
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasCreatorUserId creatorUserId)
            {
                creatorUserId.CreatorUserId = (long?)_session;
                //creatorUserId.CreatorUserId = long.Parse(_httpContextAccessor.HttpContext!.Session.Get("SessionId")?.ToString()!);
            }
            await _dbContext.Set<TEntity>().AddAsync(entry.Entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasLastModifierUserId lastModifierUserId)
            {
                //lastModifierUserId.LastModifierUserId = User.GetUserId();
                if (entry.State == EntityState.Detached)
                {
                    _dbContext.Update(entry.Entity);
                }
                else
                {
                    entry.State = EntityState.Modified;
                }
            }
            entry.State = EntityState.Modified;
            _dbContext.Update(entry.Entity);
            await _dbContext.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(id);
            if (entity == null)
            {
                return;
            }

            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is ISoftDelete deleteUserId)
            {
                //deleteUserId.DeleteUserId = User.GetUserId();
            }

            _dbContext.Set<TEntity>().Remove(entry.Entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> InsertAndGetIdAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasCreatorUserId creatorUserId)
            {
                //creatorUserId.CreatorUserId = User.GetUserId();
            }
            TEntity e = (await _dbContext.Set<TEntity>().AddAsync(entry.Entity)).Entity;
            await _dbContext.SaveChangesAsync();
            return e.Id;
        }
    }
}