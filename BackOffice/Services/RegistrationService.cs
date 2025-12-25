using BackOffice.Data;
using BackOffice.Models;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Services
{
    public class RegistrationService
    {
        private readonly AppDbContext _context;

        public RegistrationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task Create(int userId, RegistrationType status)
        {
            var user = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!user)
            {
                throw new Exception("Utilisateur introuvable");
            }

            var registration = new Registration
            {
                UserId = userId,
                Status = status,
                Timestamp = DateTime.UtcNow
            };

            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();
        }
        
    }
}