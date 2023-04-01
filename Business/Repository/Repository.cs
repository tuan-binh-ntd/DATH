using API.Data;
using API.Entities;
using API.Extensions;
using API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace API.FactoryRepository
{
    public class Repository<TEntity, TPrimaryKey> : ControllerBase, IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
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
            if (entry.Entity is IHasCreatorUserId creatorUserId && entry.State == EntityState.Added)
            {
                creatorUserId.CreatorUserId = User.GetUserId();
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
                lastModifierUserId.LastModifierUserId = User.GetUserId();
            }
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Update(entry.Entity);
                await _dbContext.SaveChangesAsync();
                return entry.Entity;
            }
            entry.State = EntityState.Modified;
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
                deleteUserId.DeleteUserId = User.GetUserId();
            }

            _dbContext.Set<TEntity>().Remove(entry.Entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasCreatorUserId creatorUserId && entry.State == EntityState.Added)
            {
                creatorUserId.CreatorUserId = User.GetUserId();
            }
            TEntity e = (await _dbContext.Set<TEntity>().AddAsync(entry.Entity)).Entity;
            await _dbContext.SaveChangesAsync();
            return e.Id;
        }
    }

    public class Repository<TEntity> : ControllerBase, IRepository<TEntity> where TEntity : class, IEntity<int>
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

        public async Task<TEntity?> GetAsync(int id)
        {
            TEntity? entity = await _dbContext.Set<TEntity>().FindAsync(id);
            return entity;
        }

        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasCreatorUserId creatorUserId && entry.State == EntityState.Added)
            {
                creatorUserId.CreatorUserId = User.GetUserId();
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
                lastModifierUserId.LastModifierUserId = User.GetUserId();
            }
            if (entry.State == EntityState.Detached)
            {
                _dbContext.Update(entry.Entity);
                await _dbContext.SaveChangesAsync();
                return entry.Entity;
            }
            entry.State = EntityState.Modified;
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
            if (entry.Entity is ISoftDelete deleteUserId && entry.State == EntityState.Deleted)
            {
                deleteUserId.DeleteUserId = User.GetUserId();
            }

            _dbContext.Set<TEntity>().Remove(entry.Entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> InsertAndGetIdAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasCreatorUserId creatorUserId && entry.State == EntityState.Added)
            {
                creatorUserId.CreatorUserId = User.GetUserId();
            }
            TEntity e = (await _dbContext.Set<TEntity>().AddAsync(entry.Entity)).Entity;
            await _dbContext.SaveChangesAsync();
            return e.Id;
        }
    }
}
