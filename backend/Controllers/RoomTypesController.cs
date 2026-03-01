using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingEngine.Data;
using HotelBookingEngine.DTOs.RoomTypes;
using HotelBookingEngine.Constants;
using System.Text.Json;

namespace HotelBookingEngine.Controllers;

[ApiController]
[Route("api/room-types")]
public class RoomTypesController(HotelDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roomTypes = await context.RoomTypes
            .Where(rt => rt.IsActive)
            .Include(rt => rt.Property)
            .ToListAsync();

        var ratePlans = await context.RatePlans
            .Where(rp => roomTypes.Select(rt => rt.Id).Contains(rp.RoomTypeId))
            .ToListAsync();

        var response = roomTypes.Select(rt => new RoomTypeResponse
        {
            Id = rt.Id,
            Name = rt.Name,
            Description = rt.Description,
            MaxAdults = rt.MaxAdults,
            MaxChildren = rt.MaxChildren,
            BaseRate = rt.BaseRate,
            Amenities = ParseJson(rt.Amenities),
            Photos = ParseJson(rt.Photos),
            RatePlans = ratePlans
                .Where(rp => rp.RoomTypeId == rt.Id)
                .Select(rp => new RatePlanSummary
                {
                    Id = rp.Id,
                    Name = rp.Name,
                    RatePerNight = rp.RatePerNight,
                    IsRefundable = rp.IsRefundable,
                    MealPlan = rp.MealPlan,
                    MinLos = rp.MinLos,
                    MaxLos = rp.MaxLos
                }).ToList()
        });

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var rt = await context.RoomTypes
            .FirstOrDefaultAsync(r => r.Id == id && r.IsActive);

        if (rt == null)
            return NotFound(new { message = ErrorMessages.RoomTypeNotFound });

        var ratePlans = await context.RatePlans.Where(rp => rp.RoomTypeId == id).ToListAsync();

        var response = new RoomTypeResponse
        {
            Id = rt.Id,
            Name = rt.Name,
            Description = rt.Description,
            MaxAdults = rt.MaxAdults,
            MaxChildren = rt.MaxChildren,
            BaseRate = rt.BaseRate,
            Amenities = ParseJson(rt.Amenities),
            Photos = ParseJson(rt.Photos),
            RatePlans = ratePlans.Select(rp => new RatePlanSummary
            {
                Id = rp.Id,
                Name = rp.Name,
                RatePerNight = rp.RatePerNight,
                IsRefundable = rp.IsRefundable,
                MealPlan = rp.MealPlan,
                MinLos = rp.MinLos,
                MaxLos = rp.MaxLos
            }).ToList()
        };

        return Ok(response);
    }

    private static List<string> ParseJson(string json)
    {
        if (string.IsNullOrEmpty(json)) return [];
        try { return JsonSerializer.Deserialize<List<string>>(json) ?? []; }
        catch { return []; }
    }
}
