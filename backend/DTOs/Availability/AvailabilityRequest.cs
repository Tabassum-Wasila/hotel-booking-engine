namespace HotelBookingEngine.DTOs.Availability
{
    public class AvailabilityRequest
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Adults { get; set; } = 1;
        public int Children { get; set; } = 0;
        public int? RoomTypeId { get; set; }
    }
}
