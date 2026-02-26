using HotelBookingEngine.Models;
using HotelBookingEngine.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingEngine.Data;

public static class SeedData
{
    public static async Task Initialize(HotelDbContext context)
    {
        if (await context.Properties.AnyAsync())
        {
            return; // database already seeded
        }

        var adminRole = new Role { Name = "Admin", Description = "Full system access" };
        var agentRole = new Role { Name = "Agent", Description = "Front desk operations" };
        
        context.Roles.AddRange(adminRole, agentRole);
        await context.SaveChangesAsync();

        var property = new Property
        {
            Name = "Hotel Premier Cyberjaya",
            Address = "Cyberjaya, Malaysia",
            Timezone = "Asia/Kuala_Lumpur",
            CheckInTime = new TimeSpan(15, 0, 0),
            CheckOutTime = new TimeSpan(11, 0, 0),
            CreatedAt = DateTime.UtcNow
        };
        
        context.Properties.Add(property);
        await context.SaveChangesAsync();

        var standard = new RoomType
        {
            PropertyId = property.Id,
            Name = "Standard Room",
            Description = "Cozy room with city view, perfect for solo travelers or couples",
            MaxAdults = 2,
            MaxChildren = 1,
            BaseRate = 120.00m,
            Amenities = "[\"wifi\", \"tv\", \"ac\"]",
            Photos = "[\"https://picsum.photos/800/600?random=1\"]",
            IsActive = true
        };

        var deluxeKing = new RoomType
        {
            PropertyId = property.Id,
            Name = "Deluxe King",
            Description = "Spacious room with king bed and ocean view",
            MaxAdults = 2,
            MaxChildren = 2,
            BaseRate = 180.00m,
            Amenities = "[\"wifi\", \"tv\", \"ac\", \"minibar\", \"balcony\"]",
            Photos = "[\"https://picsum.photos/800/600?random=2\"]",
            IsActive = true
        };

        var suite = new RoomType
        {
            PropertyId = property.Id,
            Name = "Executive Suite",
            Description = "Luxury suite with separate living area and premium amenities",
            MaxAdults = 3,
            MaxChildren = 2,
            BaseRate = 300.00m,
            Amenities = "[\"wifi\", \"tv\", \"ac\", \"minibar\", \"balcony\", \"jacuzzi\", \"kitchenette\"]",
            Photos = "[\"https://picsum.photos/800/600?random=3\"]",
            IsActive = true
        };

        var familyRoom = new RoomType
        {
            PropertyId = property.Id,
            Name = "Family Room",
            Description = "Spacious room with two queen beds, ideal for families",
            MaxAdults = 4,
            MaxChildren = 3,
            BaseRate = 220.00m,
            Amenities = "[\"wifi\", \"tv\", \"ac\", \"microwave\", \"fridge\"]",
            Photos = "[\"https://picsum.photos/800/600?random=4\"]",
            IsActive = true
        };

        context.RoomTypes.AddRange(standard, deluxeKing, suite, familyRoom);
        await context.SaveChangesAsync();

        var ratePlans = new List<RatePlan>();
        foreach (var roomType in new[] { standard, deluxeKing, suite, familyRoom })
        {
            ratePlans.Add(new RatePlan
            {
                RoomTypeId = roomType.Id,
                Name = "Flexible Rate",
                RatePerNight = roomType.BaseRate,
                IsRefundable = true,
                MealPlan = "Room Only",
                MinLos = 1,
                MaxLos = 30,
                ValidFrom = new DateTime(2026, 2, 20),
                ValidTo = new DateTime(2026, 12, 31)
            });

            ratePlans.Add(new RatePlan
            {
                RoomTypeId = roomType.Id,
                Name = "Non-Refundable",
                RatePerNight = roomType.BaseRate * 0.85m, // 15% discount
                IsRefundable = false,
                MealPlan = "Room Only",
                MinLos = 1,
                MaxLos = 30,
                ValidFrom = new DateTime(2026, 2, 20),
                ValidTo = new DateTime(2026, 12, 31)
            });

            ratePlans.Add(new RatePlan
            {
                RoomTypeId = roomType.Id,
                Name = "Breakfast Included",
                RatePerNight = roomType.BaseRate + 25.00m,
                IsRefundable = true,
                MealPlan = "Breakfast",
                MinLos = 2,
                MaxLos = 30,
                ValidFrom = new DateTime(2026, 2, 20),
                ValidTo = new DateTime(2026, 12, 31)
            });
        }

        context.RatePlans.AddRange(ratePlans);
        await context.SaveChangesAsync();

        // Seed Inventory (90 days from Feb 26 - May 26, 2026)
        var inventoryList = new List<Inventory>();
        var startDate = new DateTime(2026, 2, 26);
        var roomTypes = new[] { standard, deluxeKing, suite, familyRoom };
        var roomCounts = new[] { 10, 8, 3, 5 }; // Total rooms per type

        for (int i = 0; i < 90; i++)
        {
            var date = startDate.AddDays(i);
            for (int j = 0; j < roomTypes.Length; j++)
            {
                inventoryList.Add(new Inventory
                {
                    RoomTypeId = roomTypes[j].Id,
                    Date = date,
                    TotalRooms = roomCounts[j],
                    BookedRooms = 0
                });
            }
        }

        context.Inventories.AddRange(inventoryList);
        await context.SaveChangesAsync();

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Password123!", 12);

        var adminUser = new User
        {
            Name = "Admin User",
            Email = "admin@hotel.com",
            PasswordHash = passwordHash,
            RoleId = adminRole.Id,
            IsActive = true
        };

        var agentUser = new User
        {
            Name = "Front Desk Agent",
            Email = "agent@hotel.com",
            PasswordHash = passwordHash,
            RoleId = agentRole.Id,
            IsActive = true
        };

        context.Users.AddRange(adminUser, agentUser);
        await context.SaveChangesAsync();

        var reservations = new List<Reservation>
        {
            new Reservation
            {
                Reference = "ABC12345",
                PropertyId = property.Id,
                RoomTypeId = deluxeKing.Id,
                RatePlanId = ratePlans.First(r => r.RoomTypeId == deluxeKing.Id && r.Name == "Flexible Rate").Id,
                CheckIn = new DateTime(2026, 3, 15),
                CheckOut = new DateTime(2026, 3, 18),
                Nights = 3,
                Adults = 2,
                Children = 0,
                GuestName = "John Smith",
                GuestEmail = "john.smith@email.com",
                GuestPhone = "+1-555-0101",
                SpecialRequests = "Late check-in requested",
                NightlyRate = 180.00m,
                TaxAmount = 54.00m,
                TotalAmount = 594.00m,
                Status = ReservationStatus.CONFIRMED
            },
            
            new Reservation
            {
                Reference = "XYZ98765",
                PropertyId = property.Id,
                RoomTypeId = suite.Id,
                RatePlanId = ratePlans.First(r => r.RoomTypeId == suite.Id && r.Name == "Breakfast Included").Id,
                CheckIn = new DateTime(2026, 3, 20),
                CheckOut = new DateTime(2026, 3, 25),
                Nights = 5,
                Adults = 2,
                Children = 1,
                GuestName = "Emma Johnson",
                GuestEmail = "emma.j@email.com",
                GuestPhone = "+1-555-0202",
                SpecialRequests = "Crib needed for infant",
                NightlyRate = 325.00m,
                TaxAmount = 162.50m,
                TotalAmount = 1787.50m,
                Status = ReservationStatus.CONFIRMED
            },

            new Reservation
            {
                Reference = "DEF45678",
                PropertyId = property.Id,
                RoomTypeId = familyRoom.Id,
                RatePlanId = ratePlans.First(r => r.RoomTypeId == familyRoom.Id && r.Name == "Flexible Rate").Id,
                CheckIn = new DateTime(2026, 4, 1),
                CheckOut = new DateTime(2026, 4, 5),
                Nights = 4,
                Adults = 3,
                Children = 2,
                GuestName = "Michael Brown",
                GuestEmail = "m.brown@email.com",
                GuestPhone = "+1-555-0303",
                SpecialRequests = "",
                NightlyRate = 220.00m,
                TaxAmount = 88.00m,
                TotalAmount = 968.00m,
                Status = ReservationStatus.MODIFIED
            },

            new Reservation
            {
                Reference = "GHI78901",
                PropertyId = property.Id,
                RoomTypeId = standard.Id,
                RatePlanId = ratePlans.First(r => r.RoomTypeId == standard.Id && r.Name == "Non-Refundable").Id,
                CheckIn = new DateTime(2026, 3, 10),
                CheckOut = new DateTime(2026, 3, 12),
                Nights = 2,
                Adults = 1,
                Children = 0,
                GuestName = "Sarah Wilson",
                GuestEmail = "s.wilson@email.com",
                GuestPhone = "+1-555-0404",
                SpecialRequests = "",
                NightlyRate = 102.00m,
                TaxAmount = 20.40m,
                TotalAmount = 224.40m,
                Status = ReservationStatus.CANCELLED,
                CancelledAt = DateTime.UtcNow.AddDays(-5),
                CancellationRef = "CXL12345"
            },

            new Reservation
            {
                Reference = "JKL23456",
                PropertyId = property.Id,
                RoomTypeId = deluxeKing.Id,
                RatePlanId = ratePlans.First(r => r.RoomTypeId == deluxeKing.Id && r.Name == "Breakfast Included").Id,
                CheckIn = new DateTime(2026, 2, 25),
                CheckOut = new DateTime(2026, 2, 28),
                Nights = 3,
                Adults = 2,
                Children = 1,
                GuestName = "David Lee",
                GuestEmail = "david.lee@email.com",
                GuestPhone = "+1-555-0505",
                SpecialRequests = "High floor preferred",
                NightlyRate = 205.00m,
                TaxAmount = 61.50m,
                TotalAmount = 676.50m,
                Status = ReservationStatus.CHECKED_IN
            }
        };

        context.Reservations.AddRange(reservations);
        await context.SaveChangesAsync();

        foreach (var reservation in reservations.Where(r => r.Status == ReservationStatus.CONFIRMED || r.Status == ReservationStatus.CHECKED_IN))
        {
            for (var date = reservation.CheckIn; date < reservation.CheckOut; date = date.AddDays(1))
            {
                var inventory = await context.Inventories
                    .FirstOrDefaultAsync(i => i.RoomTypeId == reservation.RoomTypeId && i.Date == date);
                
                if (inventory != null)
                {
                    inventory.BookedRooms++;
                }
            }
        }

        await context.SaveChangesAsync();
    }
}