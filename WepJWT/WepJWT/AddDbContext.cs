using Microsoft.EntityFrameworkCore;

namespace WepJWT
{
    public class AddDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserDto> UsersDto { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data source = data.db");
        }
    }
}
