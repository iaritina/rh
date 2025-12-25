using BackOffice.Services;
using BackOffice.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Controllers
{
    public class WorkDurationController : Controller
    {
        
        private readonly RegistrationService _registrationService;

        public WorkDurationController(RegistrationService registrationService)
        {
            _registrationService = registrationService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date)
        {
            var durations = await _registrationService.GetWorkDurationByDateWithSchedule(date);
            return View(durations);
        }
    }
}