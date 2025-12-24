using BackOffice.Data;
using BackOffice.Models;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Services;

public class CongeService
{
    private readonly AppDbContext _context;

    public CongeService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<(List<Conge> Items, int TotalCount)>
        GetAllPagedAsync(int page, int pageSize)
    {
        var query = _context.Conges
            .Include(s => s.Employe)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(s => s.IdEmploye)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<Conge>> GetAllAsync(int page, int pageSize)
    {
        return await _context.Conges
            .Include(s => s.Employe)
            .ToListAsync();
    }

    public async Task<Conge?> GetByEmployeeAsync(int employeeId)
    {
        return await _context.Conges
            .FirstOrDefaultAsync(s => s.IdEmploye == employeeId);
    }

    public async Task CreateAsync(int employeeId, decimal soldeInitial = 30)
    {
        var solde = new Conge
        {
            IdEmploye = employeeId,
            SoldeRestant = soldeInitial
        };

        _context.Conges.Add(solde);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateSoldeAsync(int id, decimal nouveauSolde)
    {
        var solde = await _context.Conges.FindAsync(id);
        if (solde == null)
            throw new Exception("Solde introuvable");

        solde.SoldeRestant = nouveauSolde;
        await _context.SaveChangesAsync();
    }

    public async Task SupprimerAsync(int id)
    {
        var solde = await _context.Conges.FindAsync(id);
        if (solde == null)
            return;

        _context.Conges.Remove(solde);
        await _context.SaveChangesAsync();
    }
}