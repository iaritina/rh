using Microsoft.EntityFrameworkCore;
using BackOffice.Models;

namespace BackOffice.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Registration> Registrations { get; set; }
    }
}