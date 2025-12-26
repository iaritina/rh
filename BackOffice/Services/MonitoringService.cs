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

            int present = 0;
            int absent = 0;

            foreach (var user in users)
            {
                var lastRegistration = user.Registrations
                    .Where(r => r.Timestamp.Date == today)
                    .OrderByDescending(r => r.Timestamp)
                    .FirstOrDefault();

                var status = lastRegistration?.Status ?? RegistrationType.Exit;

                if (status == RegistrationType.Enter)
                    present++;
                else
                    absent++;

                userStatusList.Add(new UserStatusViewModel
                {
                    LastName = user.LastName,
                    LastRegistrationTime = lastRegistration?.Timestamp,
                    LastStatus = status
                });
            }

            return new MonitoringViewModel
            {
                Users = userStatusList,
                TotalUsers = users.Count,
                PresentUsers = present,
                AbsentUsers = absent
            };
        }

    }
}