using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingEngine.Data;
using HotelBookingEngine.DTOs.Property;

namespace HotelBookingEngine.Controllers;

[ApiController]
[Route("api/property")]
public class PropertyController(HotelDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var property = await context.Properties.FirstOrDefaultAsync();

        if (property == null)
            return NotFound(new { message = "No property configured" });

        return Ok(new PropertyResponse
        {
            Id = property.Id,
            Name = property.Name,
            Address = property.Address,
            Timezone = property.Timezone,
            CheckInTime = property.CheckInTime.ToString(@"hh\:mm"),
            CheckOutTime = property.CheckOutTime.ToString(@"hh\:mm")
        });
    }
}
