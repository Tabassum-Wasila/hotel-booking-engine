namespace HotelBookingEngine.Constants;

public static class ErrorMessages
{
    public const string CheckInMustBeTodayOrFuture = "Check-in date must be today or in the future";
    public const string CheckOutMustBeAfterCheckIn = "Check-out date must be after check-in date";
    public const string MaximumStay30Nights = "Maximum stay is 30 nights";
    
    public const string InvalidRoomType = "Invalid room type";
    public const string RoomTypeNotFound = "Room type not found";
    public const string RoomCannotAccommodateGuests = "Room cannot accommodate this many guests";
    
    public const string InvalidRatePlan = "Invalid rate plan for this room type";
    public const string RatePlanNotValidForDates = "Rate plan not valid for selected dates";
    
    public const string NoInventoryForSomeDates = "No inventory available for some dates";
    
    public const string NoPropertyConfigured = "No property configured";
    
    public static string RoomTypeMaxAdults(int maxAdults) => 
        $"Room type supports maximum {maxAdults} adults";
    
    public static string StayMustBeBetweenNights(int minLos, int maxLos) => 
        $"Stay must be between {minLos} and {maxLos} nights for this rate plan";
    
    public static string NoRoomsAvailableOnDate(DateTime date) => 
        $"No rooms available for {date:yyyy-MM-dd}";
}
