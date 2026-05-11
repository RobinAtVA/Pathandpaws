using Microsoft.EntityFrameworkCore;
using PathAndPaws.Models;

namespace PathAndPaws.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Lead> Leads => Set<Lead>();
        public DbSet<Article> Articles => Set<Article>();
    }
}