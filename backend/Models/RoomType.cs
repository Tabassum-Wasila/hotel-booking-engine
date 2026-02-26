namespace HotelBookingEngine.Models
{
    public class RoomType
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public int MaxAdults { get; set; }
        public int MaxChildren { get; set; }
        public decimal BaseRate { get; set; }
        public string Amenities { get; set; } = string.Empty; // JSON string
        public string Photos { get; set; } = string.Empty; // Urls as JSON string
        public bool IsActive { get; set; }

        public Property Property { get; set; } = null!;

    }
}