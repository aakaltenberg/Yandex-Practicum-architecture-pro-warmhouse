using Microsoft.EntityFrameworkCore;
using GateService.Models;
using GateService.Data;

namespace GateService.Repositories;

public interface IGateRepository
{
    Task<List<Gate>> GetAllAsync();
    Task<Gate?> GetByIdAsync(int id);
    Task<Gate> CreateAsync(Gate gate);
    Task<Gate?> UpdateAsync(int id, Gate updatedGate);
    Task<bool> DeleteAsync(int id);
    Task<bool> UpdateStatusAsync(int id, string status);
}

public class GateRepository : IGateRepository
{
    private readonly AppDbContext _context;

    public GateRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Gate>> GetAllAsync() => await _context.Gates.ToListAsync();

    public async Task<Gate?> GetByIdAsync(int id) => await _context.Gates.FindAsync(id);

    public async Task<Gate> CreateAsync(Gate gate)
    {
        gate.CreatedAt = DateTime.UtcNow;
        gate.LastUpdated = DateTime.UtcNow;
        _context.Gates.Add(gate);
        await _context.SaveChangesAsync();
        return gate;
    }

    public async Task<Gate?> UpdateAsync(int id, Gate updatedGate)
    {
        var gate = await _context.Gates.FindAsync(id);
        if (gate == null) return null;

        gate.Name = updatedGate.Name ?? gate.Name;
        gate.Location = updatedGate.Location ?? gate.Location;
        gate.Description = updatedGate.Description ?? gate.Description;
        gate.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return gate;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var gate = await _context.Gates.FindAsync(id);
        if (gate == null) return false;

        _context.Gates.Remove(gate);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateStatusAsync(int id, string status)
    {
        var gate = await _context.Gates.FindAsync(id);
        if (gate == null) return false;

        gate.Status = status;
        gate.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}