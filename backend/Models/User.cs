namespace HotelBookingEngine.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }

        public Role Role { get; set; } = null!;
    }
}