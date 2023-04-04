using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Database;
using Entities.Interface;
using Microsoft.AspNetCore.Mvc;
using Bussiness.Extensions;

namespace Bussiness.Repository
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
            if (entry.Entity is IHasCreatorUserId<TPrimaryKey> creatorUserId && entry.State == EntityState.Added)
            {
                creatorUserId.CreatorUserId = User.GetUserId<TPrimaryKey>();
                creatorUserId.CreationTime = DateTime.Now;
            }
            await _dbContext.Set<TEntity>().AddAsync(entry.Entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasLastModifierUserId<TPrimaryKey> lastModifierUserId)
            {
                lastModifierUserId.LastModifierUserId = User.GetUserId<TPrimaryKey>();
                lastModifierUserId.LastModificationTime = DateTime.Now;
                //if (entry.State == EntityState.Detached)
                //{
                //    _dbContext.Update(entry.Entity);
                //}
                //else
                //{
                //    entry.State = EntityState.Modified;
                //}
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
            if (entry.Entity is ISoftDelete<TPrimaryKey> deleteUserId && entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                deleteUserId.DeleteUserId = User.GetUserId<TPrimaryKey>();
                deleteUserId.DeletionTime = DateTime.Now;
                deleteUserId.IsDeleted = true;
            }

            _dbContext.Set<TEntity>().Update(entry.Entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasCreatorUserId<TPrimaryKey> creatorUserId && entry.State == EntityState.Added)
            {
                creatorUserId.CreatorUserId = User.GetUserId<TPrimaryKey>();
                creatorUserId.CreationTime = DateTime.Now;
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
            if (entry.Entity is IHasCreatorUserId<int> creatorUserId && entry.State == EntityState.Added)
            {
                creatorUserId.CreatorUserId = User.GetUserId<int>();
                creatorUserId.CreationTime = DateTime.Now;
            }
            await _dbContext.Set<TEntity>().AddAsync(entry.Entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasLastModifierUserId<int> lastModifierUserId)
            {
                lastModifierUserId.LastModifierUserId = User.GetUserId<int>();
                lastModifierUserId.LastModificationTime = DateTime.Now;
                //if (entry.State == EntityState.Detached)
                //{
                //    _dbContext.Update(entry.Entity);
                //}
                //else
                //{
                //    entry.State = EntityState.Modified;
                //}
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
            if (entry.Entity is ISoftDelete<int> deleteUserId && entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                deleteUserId.DeleteUserId = User.GetUserId<int>();
                deleteUserId.DeletionTime = DateTime.Now;
                deleteUserId.IsDeleted = true;
            }

            _dbContext.Set<TEntity>().Update(entry.Entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> InsertAndGetIdAsync(TEntity entity)
        {
            EntityEntry<TEntity> entry = _dbContext.Set<TEntity>().Entry(entity);
            if (entry.Entity is IHasCreatorUserId<int> creatorUserId && entry.State == EntityState.Added)
            {
                creatorUserId.CreatorUserId = User.GetUserId<int>();
                creatorUserId.CreationTime = DateTime.Now;
            }
            TEntity e = (await _dbContext.Set<TEntity>().AddAsync(entry.Entity)).Entity;
            await _dbContext.SaveChangesAsync();
            return e.Id;
        }
    }
}