using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelBookingEngine.Data;
using HotelBookingEngine.DTOs.Reservations;
using HotelBookingEngine.Models;
using HotelBookingEngine.Models.Enums;
using HotelBookingEngine.Constants;

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

        // Normalize dates to midnight
        request.CheckIn = request.CheckIn.Date;
        request.CheckOut = request.CheckOut.Date;

        if (request.CheckIn < DateTime.Today)
            return BadRequest(new { message = ErrorMessages.CheckInMustBeTodayOrFuture });

        if (request.CheckOut <= request.CheckIn)
            return BadRequest(new { message = ErrorMessages.CheckOutMustBeAfterCheckIn });

        var nights = (request.CheckOut - request.CheckIn).Days;

        var roomType = await context.RoomTypes
            .FirstOrDefaultAsync(rt => rt.Id == request.RoomTypeId && rt.IsActive);

        if (roomType == null)
            return BadRequest(new { message = ErrorMessages.InvalidRoomType });

        if (request.Adults > roomType.MaxAdults)
            return BadRequest(new { message = ErrorMessages.RoomTypeMaxAdults(roomType.MaxAdults) });

        if (request.Adults + request.Children > roomType.MaxAdults + roomType.MaxChildren)
            return BadRequest(new { message = ErrorMessages.RoomCannotAccommodateGuests });

        var ratePlan = await context.RatePlans
            .FirstOrDefaultAsync(rp => rp.Id == request.RatePlanId && rp.RoomTypeId == request.RoomTypeId);

        if (ratePlan == null)
            return BadRequest(new { message = ErrorMessages.InvalidRatePlan });

        // Validate rate plan dates and LOS (ValidTo should cover last night, not checkout day)
        var lastNight = request.CheckOut.AddDays(-1);
        if (ratePlan.ValidFrom > request.CheckIn || ratePlan.ValidTo < lastNight)
            return BadRequest(new { message = ErrorMessages.RatePlanNotValidForDates });

        if (nights < ratePlan.MinLos || nights > ratePlan.MaxLos)
            return BadRequest(new { message = ErrorMessages.StayMustBeBetweenNights(ratePlan.MinLos, ratePlan.MaxLos) });

        var property = await context.Properties.FirstOrDefaultAsync();
        if (property == null)
            return BadRequest(new { message = ErrorMessages.NoPropertyConfigured });

        var dates = Enumerable.Range(0, nights)
            .Select(i => request.CheckIn.AddDays(i))
            .ToList();

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // Lock inventory rows to prevent race conditions using SELECT FOR UPDATE
            var placeholders = string.Join(", ", Enumerable.Range(1, nights).Select(i => $"{{{i}}}"));
            var sql = $"SELECT * FROM Inventories WHERE RoomTypeId = {{0}} AND Date IN ({placeholders}) FOR UPDATE";
            
            var parameters = new List<object> { request.RoomTypeId };
            parameters.AddRange(dates.Cast<object>());
            
            var inventory = await context.Inventories
                .FromSqlRaw(sql, parameters.ToArray())
                .ToListAsync();

            if (inventory.Count != nights)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { message = ErrorMessages.NoInventoryForSomeDates });
            }

            foreach (var inv in inventory)
            {
                if (inv.TotalRooms - inv.BookedRooms < 1)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { message = ErrorMessages.NoRoomsAvailableOnDate(inv.Date) });
                }
            }

            foreach (var inv in inventory)
            {
                inv.BookedRooms++;
            }

            var totalRate = ratePlan.RatePerNight * nights;
            var taxAmount = Math.Round(totalRate * TaxRate, 2);
            var totalAmount = Math.Round(totalRate + taxAmount, 2);

            var reference = GenerateReference();

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
            SpecialRequests = r.SpecialRequests,
            CreatedAt = DateTime.UtcNow
        };
    }
}
