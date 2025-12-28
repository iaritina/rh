using BackOffice.Services;
using BackOffice.Models;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace BackOffice.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly RegistrationService _service;
        private readonly UserService _userService;

        public RegistrationController(RegistrationService service, UserService userService)
        {
            _service = service;
            _userService = userService;
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> SearchUsers(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return Json(new List<object>());

            var users = await _userService.SearchUsersByName(query);
            var result = users.Select(u => new 
            {
                id = u.Id,
                firstName = u.FirstName,
                lastName = u.LastName,
                email = u.Email
            });

            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int userId, RegistrationType status)
        {
            try
            {
                await _service.Create(userId, status);
                TempData["SuccessRegistrationMessage"] = "Pointage effectuée avec succès";
            }
            catch (Exception ex)
            {
                TempData["ErrorRegistrationMessage"] = ex.Message;
            }

            return RedirectToAction("Create", "Registration");
        }

		
    }
}