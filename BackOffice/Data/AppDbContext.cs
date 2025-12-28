using Microsoft.EntityFrameworkCore;
using BackOffice.Models;
using Shared;

namespace BackOffice.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Conge>  Conges { get; set; }
        // public DbSet<DemandeConge> DemandeConges { get; set; }
        
        public DbSet<Schedule> Schedules { get; set; }
    }
}