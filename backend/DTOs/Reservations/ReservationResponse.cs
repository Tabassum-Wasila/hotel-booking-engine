namespace HotelBookingEngine.DTOs.Reservations
{
    public class ReservationResponse
    {
        public string Reference { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public int RatePlanId { get; set; }
        public string RatePlanName { get; set; } = string.Empty;
        public bool IsRefundable { get; set; }
        public string MealPlan { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public string GuestPhone { get; set; } = string.Empty;
        public decimal NightlyRate { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? SpecialRequests { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
