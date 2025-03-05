using Microsoft.EntityFrameworkCore;
using XInstallBotProfile.Models;

namespace XInstallBotProfile.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<StatisticType> Statistics { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("user_auth_info");
            modelBuilder.Entity<StatisticType>().ToTable("statistic_type");

        }
    }

}
