using Assignment3.Model;
using Microsoft.EntityFrameworkCore;

namespace Assignment3.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AssignRoleModel> UserRoles { get; set; }
    }
}
