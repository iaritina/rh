using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BackOffice.Services;
using BackOffice.Models;
using BackOffice.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Shared;

namespace BackOffice.Controllers
{
    
    public class UsersController : Controller
    {
        private readonly UserService _service;

        public UsersController(UserService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
        {
            var result = await _service.GetAllUsers(page, pageSize);
            return View(result);
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            ModelState.Remove("Password");
            if (!ModelState.IsValid)
            {
                var errors = string.Join(", ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                Console.WriteLine("ModelState errors: " + errors);
                var users = await _service.GetAllUsers(1, 10);
                return View("Index", users);
            }


            try
            {
                await _service.CreateUser(user);
                TempData["SuccessMessage"] = "Utilisateur créé avec succès";
                return RedirectToAction("Index"); 
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erreur lors de la création de l'utilisateur : {ex.Message}");
                var users = await _service.GetAllUsers(1, 10);
                return View("Index", users);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _service.DeleteUser(id);
                TempData["SuccessMessage"] = "Utilisateur supprimé avec succès";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erreur lors de la suppression : {ex.Message}";
            }
            
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public async Task<IActionResult> ImportCsv(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Veuillez sélectionner un fichier CSV";
                return RedirectToAction("Index");
            }

            try
            {
                int count = await _service.ImportUsersFromCsvAsync(csvFile);
                TempData["ImportSuccessMessage"] = $"{count} utilisateur(s) importé(s) avec succès";
            }
            catch (Exception ex)
            {
                TempData["ImportErrorMessage"] = "Erreur lors de l'import : " + ex.Message;
            }

            return RedirectToAction("Index");
        }


    }
}