namespace HotelBookingEngine.DTOs.Reservations
{
    public class LookupRequest
    {
        public required string Reference { get; set; }
        public required string LastName { get; set; }
    }
}
