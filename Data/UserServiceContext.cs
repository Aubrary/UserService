using Microsoft.EntityFrameworkCore;
using UserService.Entities;

namespace UserService.Data {
    public class UserServiceContext : DbContext {
        
        public UserServiceContext(DbContextOptions<UserServiceContext> options) :base (options) {
        
        }

        // Override OnModelCreating in DBContext and tell it to use Pgcrypto for GUID colloumn.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasPostgresExtension("pgcrypto");
        }

        public DbSet<User> Users {get; set;}
        
    }
}