using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Entities;

namespace Database
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, long,
        IdentityUserClaim<long>, AppUserRole, IdentityUserLogin<long>,
        IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        //Start Entity Declaration
        public DbSet<Shop> Shop { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Customer> Customer { get; set; }
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

            // One to One Relationship (AppUser, Employee)
            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.AppUser)
                .HasForeignKey<Employee>(e => e.UserId);

            // One to One Relationship (AppUser, Customer)
            modelBuilder.Entity<AppUser>()
            .HasOne(u => u.Customer)
            .WithOne(c => c.AppUser)
            .HasForeignKey<Customer>(c => c.UserId);

            // One to Many Relationship (Shop, Employee)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Shop)
                .WithMany(s => s.Employees)
                .HasForeignKey(e => e.ShopId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        // Set SoftDelete
        //public override int SaveChanges()
        //{
        //    foreach (var entry in ChangeTracker.Entries())
        //    {
        //        if (entry.Entity is ISoftDelete softDeleteEntity && entry.State == EntityState.Deleted)
        //        {
        //            entry.State = EntityState.Modified;
        //            softDeleteEntity.DeletionTime = DateTime.Now;
        //            softDeleteEntity.IsDeleted = true;
        //        }
        //    }
        //    return base.SaveChanges();
        //}

        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    foreach (var entry in ChangeTracker.Entries())
        //    {
        //        if (entry.Entity is ISoftDelete softDeleteEntity && entry.State == EntityState.Deleted)
        //        {
        //            entry.State = EntityState.Modified;
        //            softDeleteEntity.DeletionTime = DateTime.Now;
        //            softDeleteEntity.IsDeleted = true;
        //        }
        //    }
        //    return await base.SaveChangesAsync(cancellationToken);
        //}
    }
}