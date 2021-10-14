using ApplicationCoreIdentity.Models.Model.Administration;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreIdentity.Models
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; set; }
    }
}
