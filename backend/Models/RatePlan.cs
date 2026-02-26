namespace HotelBookingEngine.Models
{
    public class RatePlan
    {
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public required string Name { get; set; }
        public decimal RatePerNight { get; set; }
        public bool IsRefundable { get; set; } = true; // Default to refundable
        public string MealPlan { get; set; } = "Room Only"; // Default to Room Only
        public int MinLos { get; set; } = 1; // Default to minimum 1 night
        public int MaxLos { get; set; } = 30; // Default to maximum 30 nights
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public RoomType RoomType { get; set; } = null!;
    }
}