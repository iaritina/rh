using BackOffice.Models;
using BackOffice.Data;
using BackOffice.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Services
{
    public class MonitoringService
    {
        private readonly AppDbContext _context;

        public MonitoringService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MonitoringViewModel> GetStatusAsync()
        {
            var today = DateTime.Today;

            var users = await _context.Users
                .Include(u => u.Registrations)
                .ToListAsync();

            var present = new List<User>();
            var absent = new List<User>();

            foreach (var user in users)
            {
                var lastToday = user.Registrations
                    .Where(r => r.Timestamp.Date == today)
                    .OrderByDescending(r => r.Timestamp)
                    .FirstOrDefault();

                if (lastToday != null && lastToday.Status == RegistrationType.Enter)
                    present.Add(user);
                else
                    absent.Add(user);
            }

            return new MonitoringViewModel
            {
                Present = present,
                Absent = absent
            };
        }
    }
}