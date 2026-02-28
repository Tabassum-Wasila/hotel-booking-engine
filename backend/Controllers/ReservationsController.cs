using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingEngine.Data;
using HotelBookingEngine.DTOs.Reservations;
using HotelBookingEngine.Models;
using HotelBookingEngine.Models.Enums;

namespace HotelBookingEngine.Controllers;

[ApiController]
[Route("api/reservations")]
public class ReservationsController(HotelDbContext context) : ControllerBase
{
    private const decimal TaxRate = 0.10m;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationRequest request)
    {
        // Idempotency check
        if (!string.IsNullOrEmpty(request.IdempotencyKey))
        {
            var existing = await context.Reservations
                .FirstOrDefaultAsync(r => r.Reference == request.IdempotencyKey);
            if (existing != null)
                return Ok(MapToResponse(existing));
        }

        // Validate dates
        if (request.CheckIn < DateTime.Today)
            return BadRequest(new { message = "Check-in date must be today or in the future" });

        if (request.CheckOut <= request.CheckIn)
            return BadRequest(new { message = "Check-out date must be after check-in date" });

        var nights = (request.CheckOut - request.CheckIn).Days;

        // Get room type
        var roomType = await context.RoomTypes
            .FirstOrDefaultAsync(rt => rt.Id == request.RoomTypeId && rt.IsActive);

        if (roomType == null)
            return BadRequest(new { message = "Invalid room type" });

        // Validate capacity
        if (request.Adults > roomType.MaxAdults)
            return BadRequest(new { message = $"Room type supports maximum {roomType.MaxAdults} adults" });

        if (request.Adults + request.Children > roomType.MaxAdults + roomType.MaxChildren)
            return BadRequest(new { message = "Room cannot accommodate this many guests" });

        // Get rate plan
        var ratePlan = await context.RatePlans
            .FirstOrDefaultAsync(rp => rp.Id == request.RatePlanId && rp.RoomTypeId == request.RoomTypeId);

        if (ratePlan == null)
            return BadRequest(new { message = "Invalid rate plan for this room type" });

        // Validate rate plan dates and LOS
        if (ratePlan.ValidFrom > request.CheckIn || ratePlan.ValidTo < request.CheckOut)
            return BadRequest(new { message = "Rate plan not valid for selected dates" });

        if (nights < ratePlan.MinLos || nights > ratePlan.MaxLos)
            return BadRequest(new { message = $"Stay must be between {ratePlan.MinLos} and {ratePlan.MaxLos} nights for this rate plan" });

        // Get property
        var property = await context.Properties.FirstOrDefaultAsync();
        if (property == null)
            return BadRequest(new { message = "No property configured" });

        // Get dates for the stay
        var dates = Enumerable.Range(0, nights)
            .Select(i => request.CheckIn.AddDays(i))
            .ToList();

        // Check and update inventory within a transaction
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // Lock and check inventory for all dates
            var inventory = await context.Inventories
                .Where(i => i.RoomTypeId == request.RoomTypeId)
                .Where(i => dates.Contains(i.Date))
                .ToListAsync();

            if (inventory.Count != nights)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { message = "No inventory available for some dates" });
            }

            // Check availability
            foreach (var inv in inventory)
            {
                if (inv.TotalRooms - inv.BookedRooms < 1)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { message = $"No rooms available for {inv.Date:yyyy-MM-dd}" });
                }
            }

            // Increment booked rooms
            foreach (var inv in inventory)
            {
                inv.BookedRooms++;
            }

            // Calculate pricing
            var totalRate = ratePlan.RatePerNight * nights;
            var taxAmount = Math.Round(totalRate * TaxRate, 2);
            var totalAmount = Math.Round(totalRate + taxAmount, 2);

            // Generate reference
            var reference = GenerateReference();

            // Create reservation
            var reservation = new Reservation
            {
                Reference = reference,
                PropertyId = property.Id,
                RoomTypeId = request.RoomTypeId,
                RatePlanId = request.RatePlanId,
                CheckIn = request.CheckIn,
                CheckOut = request.CheckOut,
                Nights = nights,
                Adults = request.Adults,
                Children = request.Children,
                GuestName = request.GuestName,
                GuestEmail = request.GuestEmail ?? string.Empty,
                GuestPhone = request.GuestPhone ?? string.Empty,
                SpecialRequests = request.SpecialRequests,
                NightlyRate = ratePlan.RatePerNight,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                Status = ReservationStatus.CONFIRMED
            };

            context.Reservations.Add(reservation);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            reservation.RoomType = roomType;
            reservation.RatePlan = ratePlan;

            return CreatedAtAction(nameof(Create), new { reference = reservation.Reference }, MapToResponse(reservation));
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static string GenerateReference()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 8)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private static ReservationResponse MapToResponse(Reservation r)
    {
        return new ReservationResponse
        {
            Reference = r.Reference,
            Status = r.Status.ToString(),
            CheckIn = r.CheckIn,
            CheckOut = r.CheckOut,
            Nights = r.Nights,
            Adults = r.Adults,
            Children = r.Children,
            RoomTypeId = r.RoomTypeId,
            RoomTypeName = r.RoomType?.Name ?? string.Empty,
            RatePlanId = r.RatePlanId,
            RatePlanName = r.RatePlan?.Name ?? string.Empty,
            IsRefundable = r.RatePlan?.IsRefundable ?? false,
            MealPlan = r.RatePlan?.MealPlan ?? string.Empty,
            GuestName = r.GuestName,
            GuestEmail = r.GuestEmail,
            GuestPhone = r.GuestPhone,
            NightlyRate = r.NightlyRate,
            TaxAmount = r.TaxAmount,
            TotalAmount = r.TotalAmount,
            SpecialRequests = r.SpecialRequests
        };
    }
}
