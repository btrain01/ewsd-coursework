using Microsoft.EntityFrameworkCore;

namespace backend_app.Context
{
    public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> contextOptions) : DbContext(contextOptions)
    {
        //public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {

        }
    }
}