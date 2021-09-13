using ApplicationCoreIdentity.Models.Model.Administration;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreIdentity.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
