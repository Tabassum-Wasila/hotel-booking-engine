namespace HotelBookingEngine.DTOs.Availability
{
    public class AvailabilityResponse
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public List<AvailableRoomType> RoomTypes { get; set; } = new();
    }

    public class AvailableRoomType
    {
        public int RoomTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
        public List<string> Amenities { get; set; } = new();
        public List<string> Photos { get; set; } = new();
        public int AvailableRooms { get; set; }
        public List<AvailableRatePlan> RatePlans { get; set; } = new();
    }

    public class AvailableRatePlan
    {
        public int RatePlanId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal RatePerNight { get; set; }
        public decimal TotalRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public bool IsRefundable { get; set; }
        public string MealPlan { get; set; } = string.Empty;
    }
}
