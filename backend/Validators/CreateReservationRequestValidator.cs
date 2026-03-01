using FluentValidation;
using HotelBookingEngine.Constants;
using HotelBookingEngine.DTOs.Reservations;

namespace HotelBookingEngine.Validators
{
    public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
    {
        public CreateReservationRequestValidator()
        {
            RuleFor(x => x.RoomTypeId)
                .GreaterThan(0).WithMessage(ErrorMessages.InvalidRoomType);

            RuleFor(x => x.RatePlanId)
                .GreaterThan(0).WithMessage(ErrorMessages.InvalidRatePlan);

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

            RuleFor(x => x.GuestName)
                .NotEmpty().WithMessage(ErrorMessages.GuestNameRequired)
                .MinimumLength(2).WithMessage(ErrorMessages.GuestNameTooShort)
                .MaximumLength(100).WithMessage(ErrorMessages.GuestNameTooLong);

            RuleFor(x => x.GuestEmail)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.GuestEmail))
                .WithMessage(ErrorMessages.InvalidEmailFormat);

            RuleFor(x => x.GuestPhone)
                .MaximumLength(20)
                .Matches(@"^[\d\s\+\-\(\)]*$").When(x => !string.IsNullOrEmpty(x.GuestPhone))
                .WithMessage(ErrorMessages.InvalidPhoneFormat);

            RuleFor(x => x)
                .Must(x => !string.IsNullOrEmpty(x.GuestEmail) || !string.IsNullOrEmpty(x.GuestPhone))
                .WithMessage(ErrorMessages.ContactInfoRequired);

            RuleFor(x => x.SpecialRequests)
                .MaximumLength(500).WithMessage(ErrorMessages.SpecialRequestsTooLong);

            RuleFor(x => x)
                .Must(x => (x.CheckOut - x.CheckIn).Days <= 30)
                .WithMessage(ErrorMessages.MaximumStay30Nights);
        }
    }
}
