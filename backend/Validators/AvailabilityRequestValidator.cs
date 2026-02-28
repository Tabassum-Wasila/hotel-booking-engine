using FluentValidation;
using HotelBookingEngine.DTOs.Availability;

namespace HotelBookingEngine.Validators
{
    public class AvailabilityRequestValidator : AbstractValidator<AvailabilityRequest>
    {
        public AvailabilityRequestValidator()
        {
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

            RuleFor(x => x)
                .Must(x => (x.CheckOut - x.CheckIn).Days <= 30)
                .WithMessage("Maximum stay is 30 nights");
        }
    }
}
