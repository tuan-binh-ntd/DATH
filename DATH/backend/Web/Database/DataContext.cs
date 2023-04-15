using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Entities;
using Entities.Interface;

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
        public DbSet<ProductCategory> ProductCategory { get; set; }
        public DbSet<Product> Product{ get; set; }
        public DbSet<SpecificationCategory> SpecificationCategory { get; set; }
        public DbSet<Specification> Specification { get; set; }
        public DbSet<Promotion> Promotion { get; set; }
        public DbSet<Shipping> Shipping { get; set; }
        public DbSet<Photo> Photo { get; set; }
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

            // One to Many Relationship (SpecificationCategory, Specification)
            modelBuilder.Entity<Specification>()
                .HasOne(e => e.SpecificationCategory)
                .WithMany(s => s.Specifications)
                .HasForeignKey(e => e.SpecificationCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            //Set decimal scale
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(10, 5);

            // One to Many Relationship (ProductCategory, Product)
            modelBuilder.Entity<Product>()
                .HasOne(e => e.ProductCategory)
                .WithMany(s => s.Products)
                .HasForeignKey(e => e.ProductCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            //Set decimal scale
            modelBuilder.Entity<Shipping>().Property(p => p.Cost).HasPrecision(10, 5);

            // One to Many Relationship (Product, Photo)
            modelBuilder.Entity<Photo>()
                .HasOne(e => e.Product)
                .WithMany(s => s.Photos)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
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
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}