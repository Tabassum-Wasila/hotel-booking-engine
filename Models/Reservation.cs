namespace HotelBookingEngine.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string Reference { get; set; } = string.Empty; // Unique 6-char alphanumeric, uppercase
        public int PropertyId { get; set; }
        public int RoomTypeId { get; set; }
        public int RatePlanId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public string GuestPhone { get; set; } = string.Empty;
        public string SpecialRequests { get; set; } = string.Empty;
        public decimal NightlyRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "PENDING"; // Default to PENDING, can be CONFIRMED, MODIFIED, CANCELLED, NO_SHOW, CHECKED_IN, CHECKED_OUT
        public DateTime? CancelledAt { get; set; }
        public string CancellationRef { get; set; } = string.Empty;

        public Property Property { get; set; } = null!;
        public RoomType RoomType { get; set; } = null!;
        public RatePlan RatePlan { get; set; } = null!;
    }
}