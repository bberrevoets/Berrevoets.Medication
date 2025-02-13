using Berrevoets.Medication.MedicineCatalog.Data;
using Berrevoets.Medication.MedicineCatalog.Models;
using Berrevoets.Medication.ServiceDefaults.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Berrevoets.Medication.MedicineCatalog.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MedicinesController : ControllerBase
{
    private readonly MedicineCatalogDbContext _context;

    public MedicinesController(MedicineCatalogDbContext context)
    {
        _context = context;
    }

    // GET: api/medicines
    [HttpGet]
    public async Task<IActionResult> GetMedicines()
    {
        var medicines = await _context.Medicines.ToListAsync();
        return Ok(medicines);
    }

    // GET: api/medicines/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMedicine(int id)
    {
        var medicine = await _context.Medicines.FindAsync(id);
        if (medicine == null)
            return NotFound();
        return Ok(medicine);
    }

    // POST: api/medicines
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))] // Require authentication if needed
    public async Task<IActionResult> CreateMedicine([FromBody] Medicine medicine)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        _context.Medicines.Add(medicine);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMedicine), new { id = medicine.Id }, medicine);
    }

    // PUT: api/medicines/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))] // Require authentication if needed
    public async Task<IActionResult> UpdateMedicine(int id, [FromBody] Medicine updatedMedicine)
    {
        if (id != updatedMedicine.Id) return BadRequest("Mismatched medicine id");

        _context.Entry(updatedMedicine).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (await _context.Medicines.FindAsync(id) == null) return NotFound();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/medicines/{id}
    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))] // Require authentication if needed
    public async Task<IActionResult> DeleteMedicine(int id)
    {
        var medicine = await _context.Medicines.FindAsync(id);
        if (medicine == null) return NotFound();
        _context.Medicines.Remove(medicine);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}