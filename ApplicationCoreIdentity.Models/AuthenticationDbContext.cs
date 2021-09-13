using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreIdentity.Models
{
    public class AuthenticationDbContext : IdentityDbContext
    {
        public AuthenticationDbContext() : base()
        {

        }

        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options)
        {

        }
    }
}
