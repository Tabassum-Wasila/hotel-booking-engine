using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingEngine.Data;
using HotelBookingEngine.DTOs.Availability;
using HotelBookingEngine.Constants;
using System.Text.Json;

namespace HotelBookingEngine.Controllers;

[ApiController]
[Route("api/availability")]
public class AvailabilityController(HotelDbContext context) : ControllerBase
{
    private const decimal TaxRate = 0.10m; // 10% tax

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] AvailabilityRequest request)
    {
        // Normalize dates to midnight
        request.CheckIn = request.CheckIn.Date;
        request.CheckOut = request.CheckOut.Date;

        // Validate dates
        if (request.CheckIn < DateTime.Today)
            return BadRequest(new { message = ErrorMessages.CheckInMustBeTodayOrFuture });

        if (request.CheckOut <= request.CheckIn)
            return BadRequest(new { message = ErrorMessages.CheckOutMustBeAfterCheckIn });

        var nights = (request.CheckOut - request.CheckIn).Days;
        if (nights > 30)
            return BadRequest(new { message = ErrorMessages.MaximumStay30Nights });

        var totalGuests = request.Adults + request.Children;

        // Get all dates in the range (excluding checkout date)
        var dates = Enumerable.Range(0, nights)
            .Select(i => request.CheckIn.AddDays(i))
            .ToList();

        // Get room types that can accommodate the guests
        var roomTypesQuery = context.RoomTypes
            .Where(rt => rt.IsActive)
            .Where(rt => rt.MaxAdults >= request.Adults)
            .Where(rt => rt.MaxAdults + rt.MaxChildren >= totalGuests);

        if (request.RoomTypeId.HasValue)
            roomTypesQuery = roomTypesQuery.Where(rt => rt.Id == request.RoomTypeId.Value);

        var roomTypes = await roomTypesQuery.ToListAsync();

        if (!roomTypes.Any())
            return Ok(new AvailabilityResponse
            {
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                Nights = nights,
                Adults = request.Adults,
                Children = request.Children,
                RoomTypes = []
            });

        var roomTypeIds = roomTypes.Select(rt => rt.Id).ToList();

        // Get inventory for all dates and room types
        var inventory = await context.Inventories
            .Where(i => roomTypeIds.Contains(i.RoomTypeId))
            .Where(i => dates.Contains(i.Date))
            .ToListAsync();

        // Get rate plans for these room types (valid for the date range)
        var lastNight = request.CheckOut.AddDays(-1);
        var ratePlans = await context.RatePlans
            .Where(rp => roomTypeIds.Contains(rp.RoomTypeId))
            .Where(rp => rp.ValidFrom <= request.CheckIn && rp.ValidTo >= lastNight)
            .Where(rp => rp.MinLos <= nights && rp.MaxLos >= nights)
            .ToListAsync();

        var availableRoomTypes = new List<AvailableRoomType>();

        foreach (var roomType in roomTypes)
        {
            // Get inventory for this room type across all dates
            var roomInventory = inventory
                .Where(i => i.RoomTypeId == roomType.Id)
                .ToList();

            // Must have inventory for all dates
            if (roomInventory.Count != nights)
                continue;

            // Find minimum available rooms across all dates
            var minAvailable = roomInventory.Min(i => i.TotalRooms - i.BookedRooms);

            if (minAvailable <= 0)
                continue;

            // Get rate plans for this room type
            var roomRatePlans = ratePlans
                .Where(rp => rp.RoomTypeId == roomType.Id)
                .Select(rp =>
                {
                    var totalRate = rp.RatePerNight * nights;
                    var taxAmount = totalRate * TaxRate;
                    return new AvailableRatePlan
                    {
                        RatePlanId = rp.Id,
                        Name = rp.Name,
                        RatePerNight = rp.RatePerNight,
                        TotalRate = totalRate,
                        TaxAmount = Math.Round(taxAmount, 2),
                        GrandTotal = Math.Round(totalRate + taxAmount, 2),
                        IsRefundable = rp.IsRefundable,
                        MealPlan = rp.MealPlan
                    };
                })
                .OrderBy(rp => rp.GrandTotal)
                .ToList();

            if (!roomRatePlans.Any())
                continue;

            availableRoomTypes.Add(new AvailableRoomType
            {
                RoomTypeId = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description,
                MaxAdults = roomType.MaxAdults,
                MaxChildren = roomType.MaxChildren,
                Amenities = ParseJson(roomType.Amenities),
                Photos = ParseJson(roomType.Photos),
                AvailableRooms = minAvailable,
                RatePlans = roomRatePlans
            });
        }

        return Ok(new AvailabilityResponse
        {
            CheckIn = request.CheckIn,
            CheckOut = request.CheckOut,
            Nights = nights,
            Adults = request.Adults,
            Children = request.Children,
            RoomTypes = availableRoomTypes.OrderBy(rt => rt.RatePlans.First().GrandTotal).ToList()
        });
    }

    private static List<string> ParseJson(string json)
    {
        if (string.IsNullOrEmpty(json)) return [];
        try { return JsonSerializer.Deserialize<List<string>>(json) ?? []; }
        catch { return []; }
    }
}
