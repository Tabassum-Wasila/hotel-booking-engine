namespace HotelBookingEngine.DTOs.Reservations
{
    public class ModifyReservationRequest
    {
        public required string LastName { get; set; }
        public DateTime? CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public int? RoomTypeId { get; set; }
        public int? RatePlanId { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }
        public string? SpecialRequests { get; set; }
    }
}
