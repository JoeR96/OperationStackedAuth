using Microsoft.EntityFrameworkCore;
using OperationStackedAuth.Models;

namespace OperationStackedAuth.Data
{
    public class OperationStackedAuthDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=OperationStackedAuth.db");
        }
    }
}
