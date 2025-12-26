using BackOffice.Services;
using BackOffice.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using BackOffice.Hubs;

namespace BackOffice.Controllers
{
    public class MonitoringController : Controller
    {
        private readonly MonitoringService _service;
        private readonly IHubContext<MonitoringHub> _hub;

        public MonitoringController(
            MonitoringService service,
            IHubContext<MonitoringHub> hub)
        {
            _service = service;
            _hub = hub;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _service.GetStatusAsync();
            return View(model);
        }

        // Méthode pour notifier les clients SignalR
        public async Task NotifyUpdate()
        {
            var model = await _service.GetStatusAsync();
            await _hub.Clients.All.SendAsync("RefreshStatus", model.Users.Select(u => new
            {
                u.LastName,
                LastRegistrationTime = u.LastRegistrationTime?.ToString("HH:mm"),
                StatusText = u.StatusText,
                StatusBadgeClass = u.StatusBadgeClass
            }));
        }
    }
}