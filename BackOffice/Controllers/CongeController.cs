using BackOffice.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Controllers;

public class CongeController : Controller
{
    private readonly CongeService _service;

    public CongeController(CongeService service)
    {
        _service = service;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var soldes = await _service.GetAllAsync();
        return View(soldes);
    }

    // CREATE
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int employeeId, decimal soldeRestant)
    {
        if (!ModelState.IsValid)
            return View();

        await _service.CreateAsync(employeeId, soldeRestant);
        return RedirectToAction(nameof(Index));
    }

    // EDIT
    public async Task<IActionResult> Edit(int employeeId)
    {
        var solde = await _service.GetByEmployeeAsync(employeeId);
        if (solde == null)
            return NotFound();

        return View(solde);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, decimal soldeRestant)
    {
        await _service.UpdateSoldeAsync(id, soldeRestant);
        return RedirectToAction(nameof(Index));
    }

    // DELETE
    public async Task<IActionResult> Delete(int id)
    {
        await _service.SupprimerAsync(id);
        return RedirectToAction(nameof(Index));
    }
}