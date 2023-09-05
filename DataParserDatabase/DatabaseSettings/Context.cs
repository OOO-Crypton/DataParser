using Microsoft.EntityFrameworkCore;
using DataParserDatabase.Model;

namespace DataParserDatabase.DatabaseSettings
{
    public class Context : DbContext
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public Context() : base()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql($"Server={Settings.Server}; Database={Settings.Database}; Port={Settings.Port}; " +
                $"User Id={Settings.User}; Password={Settings.Password}; CommandTimeout=120");
        }

        public DbSet<Algorithm> Algorithms { get; set; }
        public DbSet<Coin> Coins { get; set; }
        public DbSet<Monitoring> Monitorings { get; set; }
    }
}
