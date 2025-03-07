using Microsoft.EntityFrameworkCore;
using XInstallBotProfile.Models;

namespace XInstallBotProfile.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<StatisticType> StatisticTypes { get; set; }
        public DbSet<UserStatistic> UserStatistics { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("user_auth_info");
            modelBuilder.Entity<StatisticType>().ToTable("statistic_type");
            modelBuilder.Entity<UserStatistic>().ToTable("user_statistic");

            // Установка внешнего ключа для связи UserStatistic с StatisticType
            modelBuilder.Entity<UserStatistic>()
                .HasOne(us => us.StatisticType)
                .WithMany(st => st.UserStatistics)
                .HasForeignKey(us => us.StatisticTypeId);
        }
    }

}
