using FluentValidation;
using HotelBookingEngine.DTOs.Reservations;

namespace HotelBookingEngine.Validators
{
    public class ModifyReservationRequestValidator : AbstractValidator<ModifyReservationRequest>
    {
        public ModifyReservationRequestValidator()
        {
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MinimumLength(2).WithMessage("Last name must be at least 2 characters");

            RuleFor(x => x.CheckIn)
                .GreaterThanOrEqualTo(DateTime.Today)
                .When(x => x.CheckIn.HasValue)
                .WithMessage("Check-in date must be today or in the future");

            RuleFor(x => x.CheckOut)
                .GreaterThan(x => x.CheckIn ?? DateTime.MinValue)
                .When(x => x.CheckOut.HasValue && x.CheckIn.HasValue)
                .WithMessage("Check-out date must be after check-in date");

            RuleFor(x => x.RoomTypeId)
                .GreaterThan(0)
                .When(x => x.RoomTypeId.HasValue)
                .WithMessage("Valid room type is required");

            RuleFor(x => x.RatePlanId)
                .GreaterThan(0)
                .When(x => x.RatePlanId.HasValue)
                .WithMessage("Valid rate plan is required");

            RuleFor(x => x.Adults)
                .GreaterThanOrEqualTo(1)
                .When(x => x.Adults.HasValue)
                .WithMessage("At least 1 adult is required");

            RuleFor(x => x.Adults)
                .LessThanOrEqualTo(10)
                .When(x => x.Adults.HasValue)
                .WithMessage("Maximum 10 adults allowed");

            RuleFor(x => x.Children)
                .GreaterThanOrEqualTo(0)
                .When(x => x.Children.HasValue)
                .WithMessage("Children cannot be negative");

            RuleFor(x => x.Children)
                .LessThanOrEqualTo(6)
                .When(x => x.Children.HasValue)
                .WithMessage("Maximum 6 children allowed");

            RuleFor(x => x.SpecialRequests)
                .MaximumLength(500)
                .When(x => x.SpecialRequests != null)
                .WithMessage("Special requests cannot exceed 500 characters");
        }
    }
}
