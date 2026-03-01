namespace HotelBookingEngine.DTOs.Reservations
{
    public class CreateReservationRequest
    {
        public int RoomTypeId { get; set; }
        public int RatePlanId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Adults { get; set; } = 1;
        public int Children { get; set; } = 0;
        public required string GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhone { get; set; }
        public string SpecialRequests { get; set; } = string.Empty;
        public string? IdempotencyKey { get; set; }
    }
}
