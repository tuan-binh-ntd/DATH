using API.Entities;
using API.Extensions;
using API.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, long, 
        IdentityUserClaim<long>, AppUserRole, IdentityUserLogin<long>,
        IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        //Start Entity Declaration
        public DbSet<Demo> Demo { get; set; }
        //End Entity Declaration

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Remane AspNetUsers 
            modelBuilder.Entity<AppUser>().ToTable("AppUser");
            //Remane AspNetRoles 
            modelBuilder.Entity<AppRole>().ToTable("AppRole");
            //Apply BaseEntity Configuration
            modelBuilder.ApplyBaseEntityConfiguration();
            //Many to Many Relationship (AppUser, AppUserRole, AppRole)
            modelBuilder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            modelBuilder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

        }
        // Set SoftDelete
        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is IHasCreatorUserId hasCreatorUserIdEntity && entry.State == EntityState.Added)
                {
                    hasCreatorUserIdEntity.CreationTime = DateTime.Now;
                }
                else if (entry.Entity is IHasLastModifierUserId hasLastModifierUserIdEntity && entry.State == EntityState.Modified)
                {
                    hasLastModifierUserIdEntity.LastModificationTime = DateTime.Now;
                }
                else if (entry.Entity is ISoftDelete softDeleteEntity && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    softDeleteEntity.DeletionTime = DateTime.Now;
                    softDeleteEntity.IsDeleted = true;
                }
            }
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach(var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is IHasCreatorUserId hasCreatorUserIdEntity && entry.State == EntityState.Added)
                {
                    hasCreatorUserIdEntity.CreationTime = DateTime.Now;
                }
                else if (entry.Entity is IHasLastModifierUserId hasLastModifierUserIdEntity && entry.State == EntityState.Modified)
                {
                    hasLastModifierUserIdEntity.LastModificationTime = DateTime.Now;
                }
                else if (entry.Entity is ISoftDelete softDeleteEntity && entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    softDeleteEntity.DeletionTime = DateTime.Now;
                    softDeleteEntity.IsDeleted = true;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
