using Microsoft.EntityFrameworkCore;
using UserManagmentSystem_Dot.Net8.Controllers.Entityes;

namespace UserManagmentSystem_Dot.Net8.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> options)
    : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
