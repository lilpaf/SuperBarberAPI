using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistence.Entities;

namespace Persistence.Contexts
{
    public class SuperBarberDbContext : IdentityDbContext<User>
    {
        public SuperBarberDbContext(DbContextOptions<SuperBarberDbContext> options) : base(options)
        {
        }

        public DbSet<Barber> Barbers { get; set; }
        public DbSet<BarberOrder> BarberOrders { get; set; }
        public DbSet<BarberShop> BarberShops { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Barber>()
                .HasOne(b => b.User)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Barber>()
                .HasMany(b => b.Orders)
                .WithOne(bo => bo.Barber)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BarberOrder>()
                .HasKey(bo => new { bo.BarberId, bo.OrderId });

            modelBuilder.Entity<BarberOrder>()
                .HasOne(bo => bo.Barber)
                .WithMany(o => o.Orders)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<BarberOrder>()
                .HasOne(bo => bo.Order)
                .WithMany(o => o.Barbers)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BarberShop>()
                .HasMany(bs => bs.Orders)
                .WithOne(o => o.BarberShop)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<BarberShop>()
                .HasMany(bs => bs.Barbers)
                .WithOne(bsb => bsb.BarberShop)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<BarberShop>()
                .HasMany(bs => bs.Services)
                .WithOne(bss => bss.BarberShop)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<BarberShop>()
                .HasOne(bs => bs.City)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<BarberShop>()
                .HasOne(bs => bs.District)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BarberShopBarber>()
                .HasKey(bsb => new { bsb.BarberId, bsb.BarberShopId });

            modelBuilder.Entity<BarberShopBarber>()
                .HasOne(bsb => bsb.Barber)
                .WithMany(bs => bs.BarberShops)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<BarberShopBarber>()
                .HasOne(bsb => bsb.BarberShop)
                .WithMany(bs => bs.Barbers)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BarberShopService>()
                .HasKey(bss => new { bss.ServiceId, bss.BarberShopId });

            modelBuilder.Entity<BarberShopService>()
                .HasOne(bss => bss.Service)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<BarberShopService>()
                .HasOne(bss => bss.BarberShop)
                .WithMany(bs => bs.Services)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Services)
                .WithOne(sc => sc.Category)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<District>()
                .HasOne(d => d.City)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Barbers)
                .WithOne(bo => bo.Order)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Services)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(b => b.Reservations)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.BarberShop)
                .WithMany(b => b.Orders)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasMany(s => s.Categories)
                .WithOne(sc => sc.Service)    
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceCategory>()
                .HasOne(sc => sc.Category)
                .WithMany(c => c.Services)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<ServiceCategory>()
                .HasOne(sc => sc.Service)
                .WithMany(s => s.Categories)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceCategory>()
                .HasKey(sc => new { sc.ServiceId, sc.CategoryId });

            modelBuilder.Entity<User>()
                .HasMany(u => u.Reservations)
                .WithOne(r => r.User)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<UserRefreshToken>()
                .HasOne(u => u.User)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
