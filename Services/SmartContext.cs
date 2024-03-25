using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SmartApp.Models;

namespace SmartApp.Services
{
    public class SmartContext: DbContext
    {
        private readonly string _connectionString;
        public DbSet<Author> authors { get; set; } = null!;
        public DbSet<Comment> comments { get; set; } = null!;
        public DbSet<Rating> rating { get; set; } = null!;
        public DbSet<BlackList> blacklist { get; set; } = null!;

        public SmartContext(IOptions<SmartDBConnection> smartDBConnectionAccessor)
        {
            _connectionString = smartDBConnectionAccessor.Value.ConnectionString;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
}
