using BackOffice.Data;
using BackOffice.Models;
using Microsoft.EntityFrameworkCore;
using BackOffice.ViewModels;
using Microsoft.AspNetCore.SignalR;
using BackOffice.Hubs;


namespace BackOffice.Services
{
    public class RegistrationService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<MonitoringHub> _hub;
        private readonly MonitoringService _monitoringService;

        public RegistrationService(
            AppDbContext context,
            IHubContext<MonitoringHub> hub,
            MonitoringService monitoringService)
        {
            _context = context;
            _hub = hub;
            _monitoringService = monitoringService;
        }

        public async Task Create(int userId, RegistrationType status)
        {
            var exists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!exists) throw new Exception("Utilisateur introuvable");

            var registration = new Registration
            {
                UserId = userId,
                Status = status,
                Timestamp = DateTime.UtcNow
            };

            _context.Registrations.Add(registration);
            await _context.SaveChangesAsync();


            var model = await _monitoringService.GetStatusAsync();

            await _hub.Clients.All.SendAsync("RefreshStatus", new
            {
                model.TotalUsers,
                model.PresentUsers,
                model.AbsentUsers,
                Users = model.Users.Select(u => new
                {
                    u.LastName,
                    lastRegistrationTime = u.LastRegistrationTime?.ToString("HH:mm dd/MM/yyyy") ?? "-",
                    StatusText = u.StatusText,
                    StatusBadgeClass = u.StatusBadgeClass
                })
            });

        }


        public async Task<List<WorkDuration>> GetWorkDurationByDateWithSchedule(DateTime? date = null)
        {
            var targetDate = date ?? DateTime.Today;

            var registrations = await _context.Registrations
                .Where(r => r.Timestamp.Date == targetDate.Date)
                .OrderBy(r => r.UserId)
                .ThenBy(r => r.Timestamp)
                .ToListAsync();

            var result = new List<WorkDuration>();

            var groupedByUser = registrations.GroupBy(r => r.UserId);

            foreach (var group in groupedByUser)
            {
                TimeSpan total = TimeSpan.Zero;
                DateTime? entryTime = null;

                foreach (var reg in group)
                {
                    if (reg.Status == RegistrationType.Enter) 
                    {
                        entryTime = reg.Timestamp;
                    }
                    else if (reg.Status == RegistrationType.Exit && entryTime.HasValue) 
                    {
                        total += reg.Timestamp - entryTime.Value;
                        entryTime = null;
                    }

                }

                var user = await _context.Users.FindAsync(group.Key);

            
                var schedules = await _context.Schedules
                    .Where(s => s.UserId == group.Key && s.Day == (int)targetDate.DayOfWeek + 1 && s.Working)
                    .ToListAsync();

                
                TimeSpan scheduledTime = TimeSpan.Zero;
                foreach (var sch in schedules)
                {
                    if(TimeSpan.TryParse(sch.Start, out var start) && TimeSpan.TryParse(sch.End, out var end))
                    {
                        scheduledTime += end - start;
                    }
                }

                double percentage = scheduledTime.TotalMinutes > 0 ? (total.TotalMinutes / scheduledTime.TotalMinutes) * 100 : 0;

                result.Add(new WorkDuration
                {
                    UserId = group.Key,
                    LastName = user?.LastName ?? "",
                    TotalWorked = total,
                    Date = targetDate,
                    ScheduledTime = scheduledTime,
                    Percentage = Math.Round(percentage, 2)
                });
            }

            return result;
        }
        
        
    }
}