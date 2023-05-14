using Microsoft.EntityFrameworkCore;

namespace VkWallPostgre.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ProcesedPost> ProcesedPosts { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
