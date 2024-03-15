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
        public DbSet<BarberRating> BarberRatings { get; set; }
        public DbSet<BarberShop> BarberShops { get; set; }
        public DbSet<BarberShopBarber> BarberShopBarbers { get; set; }
        public DbSet<BarberShopBarberWeekend> BarberShopBarberWeekends { get; set; }
        public DbSet<BarberShopRating> BarberShopRatings { get; set; }
        public DbSet<BarberShopService> BarberShopServices { get; set; }
        public DbSet<BarberShopServiceOrder> BarberShopServicesOrders { get; set; }
        public DbSet<BarberShopWorkingDay> BarberShopWorkingDays { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderCancellationReason> OrderCancellationReasons { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Neighborhood> Neighborhoods { get; set; }
        public DbSet<UserRating> UserRatings { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
        public DbSet<WeekDay> WeekDays { get; set; }

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
 
            modelBuilder.Entity<BarberRating>()
                .HasOne(br => br.Barber)
                .WithMany(b => b.Ratings)
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
                .WithMany(c => c.BarberShops)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<BarberShop>()
                .HasOne(bs => bs.Neighborhood)
                .WithMany(n => n.BarberShops)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BarberShopBarber>()
                .HasOne(bsb => bsb.Barber)
                .WithMany(bs => bs.BarberShops)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<BarberShopBarber>()
                .HasOne(bsb => bsb.BarberShop)
                .WithMany(bs => bs.Barbers)
                .OnDelete(DeleteBehavior.Restrict); 
            
            modelBuilder.Entity<BarberShopBarber>()
                .HasMany(bsb => bsb.Weekends)
                .WithOne(bbw => bbw.BarberShopBarber)
                .OnDelete(DeleteBehavior.Restrict);
   
            modelBuilder.Entity<BarberShopBarberWeekend>()
                .HasOne(bbw => bbw.BarberShopBarber)
                .WithMany(bsb => bsb.Weekends)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BarberShopRating>()
               .HasOne(bsr => bsr.BarberShop)
               .WithMany(b => b.Ratings)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BarberShopService>()
                .HasOne(bss => bss.Service)
                .WithMany(s => s.BarberShopServices)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<BarberShopService>()
                .HasOne(bss => bss.BarberShop)
                .WithMany(bs => bs.Services)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BarberShopService>()
                .HasMany(bss => bss.Orders)
                .WithOne(bsso => bsso.BarberShopService)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<BarberShopServiceOrder>()
                .HasOne(bsso => bsso.BarberShopService)
                .WithMany(bss => bss.Orders)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<BarberShopServiceOrder>()
                .HasOne(bsso => bsso.Order)
                .WithMany(o => o.Services)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BarberShopWorkingDay>()
                .HasOne(bswd => bswd.BarberShop)
                .WithMany(bs => bs.BarberShopWorkingDays)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<BarberShopWorkingDay>()
                .HasOne(bswd => bswd.WeekDay)
                .WithMany(wd => wd.BarberShopWorkingDays)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Services)
                .WithOne(sc => sc.Category)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Neighborhood>()
                .HasOne(n => n.City)
                .WithMany(c => c.Neighborhoods)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Barber)
                .WithMany(b => b.Orders)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasMany(o => o.Services)
                .WithOne(bsso => bsso.Order)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(b => b.Reservations)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.BarberShop)
                .WithMany(b => b.Orders)
                .OnDelete(DeleteBehavior.Restrict); 
            
            modelBuilder.Entity<Order>()
                .HasOne(o => o.OrderCancellationReason)
                .WithMany(b => b.Orders)
                .OnDelete(DeleteBehavior.Restrict); 
            
            modelBuilder.Entity<OrderCancellationReason>()
                .HasMany(ocr => ocr.Orders)
                .WithOne(o => o.OrderCancellationReason)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Service>()
                .HasMany(s => s.Categories)
                .WithOne(sc => sc.Service)    
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Service>()
                .HasMany(s => s.BarberShopServices)
                .WithOne(bss => bss.Service)    
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceCategory>()
                .HasOne(sc => sc.Category)
                .WithMany(c => c.Services)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<ServiceCategory>()
                .HasOne(sc => sc.Service)
                .WithMany(s => s.Categories)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Reservations)
                .WithOne(r => r.User)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRating>()
               .HasOne(ur => ur.User)
               .WithMany(u => u.Ratings)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserRefreshToken>()
                .HasOne(urt => urt.User)
                .WithMany(u => u.RefreshTokens)
                .OnDelete(DeleteBehavior.Restrict); //ToDo may need change when we delete a user we will delete its token
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
