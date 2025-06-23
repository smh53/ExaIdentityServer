using Microsoft.EntityFrameworkCore;

namespace AuthServer.Models
{
    public class CustomDbContext : DbContext
    {
        public CustomDbContext(DbContextOptions<CustomDbContext> options)
            : base(options)
        {
        }
        public DbSet<CustomUser> CustomUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<CustomUser>().HasData(new CustomUser()
            {
                Id = 1,
                UserName = "admin",
                Email = "semih.tavukcu@outlook.com",
                Password = "admin123",
                City = "Istanbul"
            },
            new CustomUser()
            {
                Id = 2,
                UserName = "user",
                Email = "user@gmail.com",
                Password = "user123",
                City = "Ankara"
            },
            new CustomUser()
            {
                  Id = 3,
                  UserName = "customer",
                  Email = "customer@gmail.com",
                  Password = "customer123",
                  City = "Izmir"
            }

            );
            base.OnModelCreating(modelBuilder);
        }

    }
}
