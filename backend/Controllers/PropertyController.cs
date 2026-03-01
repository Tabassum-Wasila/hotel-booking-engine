
using HotelBookingEngine.DTOs.Property;
using HotelBookingEngine.Constants;

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
            return NotFound(new { message = ErrorMessages.NoPropertyConfigured });

        return Ok(new PropertyResponse
        {
            Id = property.Id,
            Name = property.Name,
            Address = property.Address,
            Timezone = property.Timezone,
            CheckInTime = property.CheckInTime.ToString(DateFormats.Time),
            CheckOutTime = property.CheckOutTime.ToString(DateFormats.Time)
        });
    }
}
