namespace HotelBookingEngine.DTOs.RoomTypes
{
    public class RoomTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
        public decimal BaseRate { get; set; }
        public List<string> Amenities { get; set; } = new();
        public List<string> Photos { get; set; } = new();
        public bool IsActive { get; set; }
        public List<RatePlanSummary> RatePlans { get; set; } = new();
    }

    public class RatePlanSummary
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal RatePerNight { get; set; }
        public bool IsRefundable { get; set; }
        public string MealPlan { get; set; } = string.Empty;
        public int MinLos { get; set; }
        public int MaxLos { get; set; }
    }
}
