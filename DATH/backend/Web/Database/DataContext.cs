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

        public DataContext(
            DbContextOptions<DataContext> options) : base(options)
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
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<Installment> Installment { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<WarehouseDetail> WarehouseDetail { get; set; }
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

            //Set decimal scale for Price col in Product tbl
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(19, 5);

            // One to Many Relationship (ProductCategory, Product)
            modelBuilder.Entity<Product>()
                .HasOne(e => e.ProductCategory)
                .WithMany(s => s.Products)
                .HasForeignKey(e => e.ProductCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            //Set decimal scale for Cost col in Shipping tbl
            modelBuilder.Entity<Shipping>().Property(p => p.Cost).HasPrecision(19, 5);

            // One to Many Relationship (Product, Photo)
            modelBuilder.Entity<Photo>()
                .HasOne(e => e.Product)
                .WithMany(s => s.Photos)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            
            //Set decimal scale for Balance col in Installment tbl
            modelBuilder.Entity<Installment>().Property(i => i.Balance).HasPrecision(19, 5);

            // Many to many Relationship (Product, WarehouseDetail, Warehouse)
            modelBuilder.Entity<Product>()
                .HasMany(ur => ur.Products)
                .WithOne(u => u.Product)
                .HasForeignKey(ur => ur.ProductId)
                .IsRequired();

            modelBuilder.Entity<Warehouse>()
                .HasMany(ur => ur.Products)
                .WithOne(u => u.Warehouse)
                .HasForeignKey(ur => ur.WarehouseId)
                .IsRequired();

            // One to Many Relationship (Specification, Photo)
            modelBuilder.Entity<Specification>()
                .HasMany(s => s.Photos)
                .WithOne(p => p.Specification)
                .HasForeignKey(p => p.SpecificationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Many to Many Relationship (Product, OrderDetail, Order)
            modelBuilder.Entity<Product>()
                .HasMany(s => s.OrderDetails)
                .WithOne(od => od.Product)
                .HasForeignKey(od => od.ProductId)
                .IsRequired();

            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .IsRequired();

            // One to Many Relationship (Promotion, Order)
            modelBuilder.Entity<Promotion>()
                .HasMany(p => p.Orders)
                .WithOne(o => o.Promotion)
                .HasForeignKey(o => o.PromotionId)
                .OnDelete(DeleteBehavior.Cascade);

            // One to Many Relationship (Shipping, Order)
            modelBuilder.Entity<Shipping>()
                .HasMany(p => p.Orders)
                .WithOne(o => o.Shipping)
                .HasForeignKey(o => o.ShippingId)
                .OnDelete(DeleteBehavior.Cascade);

            // One to Many Relationship (Shop, Order)
            modelBuilder.Entity<Shop>()
                .HasMany(p => p.Orders)
                .WithOne(o => o.Shop)
                .HasForeignKey(o => o.ShopId)
                .OnDelete(DeleteBehavior.Cascade);

            // One to Many Relationship (Installment, OrderDetail)
            modelBuilder.Entity<Installment>()
                .HasMany(i => i.OrderDetails)
                .WithOne(od => od.Installment)
                .HasForeignKey(od => od.InstallmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // One to Many Relationship (OrderDetail, InstallmentSchedule)
            modelBuilder.Entity<OrderDetail>()
                .HasMany(od => od.InstallmentSchedules)
                .WithOne(ic => ic.OrderDetail)
                .HasForeignKey(ic => ic.OrderDetailId)
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