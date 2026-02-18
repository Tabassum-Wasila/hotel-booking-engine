namespace HotelBookingEngine.Models
{
    public class Property
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Timezone { get; set; } = string.Empty;
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}