namespace HotelBookingEngine.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public DateTime Date { get; set; }
        public int TotalRooms { get; set; }
        public int BookedRooms { get; set; }

        public RoomType RoomType { get; set; } = null!;

        public bool IsValid()
        {
            return BookedRooms <= TotalRooms;
        }
    }
}
