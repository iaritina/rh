using BackOffice.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Controllers;

public class CongeController : Controller
{
    private readonly CongeService _service;
    private const int PageSize = 5;

    public CongeController(CongeService service)
    {
        _service = service;
    }
    // GET
    public async Task<IActionResult> Index(int page = 1)
    {
        var (items, totalCount) =
            await _service.GetAllPagedAsync(page, PageSize);

        var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(items);
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