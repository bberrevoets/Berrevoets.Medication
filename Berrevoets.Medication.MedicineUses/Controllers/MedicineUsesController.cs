using System.Security.Claims;
using Berrevoets.Medication.MedicineUses.Data;
using Berrevoets.Medication.MedicineUses.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Berrevoets.Medication.MedicineUses.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all endpoints
public class MedicineUsesController : ControllerBase
{
    private readonly MedicineUsesDbContext _context;

    public MedicineUsesController(MedicineUsesDbContext context)
    {
        _context = context;
    }

    // GET: api/medicineuses
    [HttpGet]
    public async Task<IActionResult> GetMedicineUses()
    {
        // Extract the user identifier from the JWT token (using the NameIdentifier claim)
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var uses = await _context.MedicineUses.Where(mu => mu.UserId == userId).ToListAsync();
        return Ok(uses);
    }

    // GET: api/medicineuses/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetMedicineUse(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var use = await _context.MedicineUses.FirstOrDefaultAsync(mu => mu.Id == id && mu.UserId == userId);
        if (use == null)
            return NotFound();
        return Ok(use);
    }

    // POST: api/medicineuses
    [HttpPost]
    public async Task<IActionResult> CreateMedicineUse([FromBody] MedicineUse medicineUse)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        // Ensure the record is tied to the authenticated user
        medicineUse.UserId = userId;

        _context.MedicineUses.Add(medicineUse);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMedicineUse), new { id = medicineUse.Id }, medicineUse);
    }

    // PUT: api/medicineuses/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateMedicineUse(int id, [FromBody] MedicineUse updatedUse)
    {
        if (id != updatedUse.Id)
            return BadRequest("ID mismatch");

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var existingUse = await _context.MedicineUses.FirstOrDefaultAsync(mu => mu.Id == id && mu.UserId == userId);
        if (existingUse == null)
            return NotFound();

        // Update properties (adjust as necessary)
        existingUse.MedicineCatalogId = updatedUse.MedicineCatalogId;
        existingUse.MedicineName = updatedUse.MedicineName;
        existingUse.DailyDose = updatedUse.DailyDose;
        existingUse.StockAtHome = updatedUse.StockAtHome;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/medicineuses/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMedicineUse(int id)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var use = await _context.MedicineUses.FirstOrDefaultAsync(mu => mu.Id == id && mu.UserId == userId);
        if (use == null)
            return NotFound();

        _context.MedicineUses.Remove(use);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}