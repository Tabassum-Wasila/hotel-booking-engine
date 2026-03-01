using FluentValidation;
using HotelBookingEngine.Constants;
using HotelBookingEngine.DTOs.Availability;

namespace HotelBookingEngine.Validators
{
    public class AvailabilityRequestValidator : AbstractValidator<AvailabilityRequest>
    {
        public AvailabilityRequestValidator()
        {
            RuleFor(x => x.CheckIn)
                .NotEmpty().WithMessage(ErrorMessages.CheckInRequired)
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage(ErrorMessages.CheckInMustBeTodayOrFuture);

            RuleFor(x => x.CheckOut)
                .NotEmpty().WithMessage(ErrorMessages.CheckOutRequired)
                .GreaterThan(x => x.CheckIn).WithMessage(ErrorMessages.CheckOutMustBeAfterCheckIn);

            RuleFor(x => x.Adults)
                .GreaterThanOrEqualTo(1).WithMessage(ErrorMessages.AdultRequired)
                .LessThanOrEqualTo(10).WithMessage(ErrorMessages.MaximumAdultsAllowed);

            RuleFor(x => x.Children)
                .GreaterThanOrEqualTo(0).WithMessage(ErrorMessages.ChildrenCannotBeNegative)
                .LessThanOrEqualTo(6).WithMessage(ErrorMessages.MaximumChildrenAllowed);

            RuleFor(x => x)
                .Must(x => (x.CheckOut - x.CheckIn).Days <= 30)
                .WithMessage(ErrorMessages.MaximumStay30Nights);
        }
    }
}
