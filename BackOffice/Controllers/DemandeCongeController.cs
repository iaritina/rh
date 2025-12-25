using BackOffice.Models;
using BackOffice.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Controllers;

public class DemandeCongeController : Controller
{
    private readonly DemandeCongeService _demandeService;
    
    private const int PageSize = 5;

    public DemandeCongeController(
        DemandeCongeService demandeCongeService)
    {
        _demandeService = demandeCongeService;
    }

    // ==========================
    // LISTE DES DEMANDES
    // ==========================
    public async Task<IActionResult> Index(int page = 1)
    {
        var (items, totalCount) =
            await _demandeService.GetAllPagedAsync(page, PageSize);

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages =
            (int)Math.Ceiling(totalCount / (double)PageSize);

        return View(items);
    }

    // ==========================
    // FORMULAIRE CREATION
    // ==========================
    public IActionResult Create()
    {
        return View();
    }

    // ==========================
    // SOUMISSION DEMANDE
    // ==========================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DemandeConge model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // Valeurs contrôlées côté serveur
        model.Status = StatusEnum.pending;

        await _demandeService.CreerDemandeAsync(model);

        return RedirectToAction(nameof(Index));
    }

    // ==========================
    // VALIDATION (RH / MANAGER)
    // ==========================
    public async Task<IActionResult> Valider(int id)
    {
        try
        {
            await _demandeService.ValiderDemandeAsync(id);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}