using FluentValidation;
using HotelBookingEngine.DTOs.Reservations;

namespace HotelBookingEngine.Validators
{
    public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
    {
        public CreateReservationRequestValidator()
        {
            RuleFor(x => x.RoomTypeId)
                .GreaterThan(0).WithMessage("Valid room type is required");

            RuleFor(x => x.RatePlanId)
                .GreaterThan(0).WithMessage("Valid rate plan is required");

            RuleFor(x => x.CheckIn)
                .NotEmpty().WithMessage("Check-in date is required")
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Check-in date must be today or in the future");

            RuleFor(x => x.CheckOut)
                .NotEmpty().WithMessage("Check-out date is required")
                .GreaterThan(x => x.CheckIn).WithMessage("Check-out date must be after check-in date");

            RuleFor(x => x.Adults)
                .GreaterThanOrEqualTo(1).WithMessage("At least 1 adult is required")
                .LessThanOrEqualTo(10).WithMessage("Maximum 10 adults allowed");

            RuleFor(x => x.Children)
                .GreaterThanOrEqualTo(0).WithMessage("Children cannot be negative")
                .LessThanOrEqualTo(6).WithMessage("Maximum 6 children allowed");

            RuleFor(x => x.GuestName)
                .NotEmpty().WithMessage("Guest name is required")
                .MinimumLength(2).WithMessage("Guest name must be at least 2 characters")
                .MaximumLength(100).WithMessage("Guest name cannot exceed 100 characters");

            RuleFor(x => x.GuestEmail)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.GuestEmail))
                .WithMessage("Valid email address is required");

            RuleFor(x => x.GuestPhone)
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters")
                .Matches(@"^[\d\s\+\-\(\)]*$").When(x => !string.IsNullOrEmpty(x.GuestPhone))
                .WithMessage("Invalid phone number format");

            RuleFor(x => x)
                .Must(x => !string.IsNullOrEmpty(x.GuestEmail) || !string.IsNullOrEmpty(x.GuestPhone))
                .WithMessage("Either email or phone is required");

            RuleFor(x => x.SpecialRequests)
                .MaximumLength(500).WithMessage("Special requests cannot exceed 500 characters");

            RuleFor(x => x)
                .Must(x => (x.CheckOut - x.CheckIn).Days <= 30)
                .WithMessage("Maximum stay is 30 nights");
        }
    }
}
