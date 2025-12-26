using BackOffice.Data;
using BackOffice.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using BackOffice.Models;

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

            var userStatusList = new List<UserStatusViewModel>();

            foreach (var user in users)
            {
                var lastRegistration = user.Registrations
                    .Where(r => r.Timestamp.Date == today)
                    .OrderByDescending(r => r.Timestamp)
                    .FirstOrDefault();

                userStatusList.Add(new UserStatusViewModel
                {
                    LastName = user.LastName,
                    LastRegistrationTime = lastRegistration?.Timestamp,
                    LastStatus = lastRegistration?.Status ?? RegistrationType.Exit
                });
            }

            return new MonitoringViewModel { Users = userStatusList };
        }
    }
}