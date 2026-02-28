using FluentValidation;
using HotelBookingEngine.DTOs.Reservations;

namespace HotelBookingEngine.Validators
{
    public class LookupRequestValidator : AbstractValidator<LookupRequest>
    {
        public LookupRequestValidator()
        {
            RuleFor(x => x.Reference)
                .NotEmpty().WithMessage("Reservation reference is required")
                .Length(6, 8).WithMessage("Invalid reservation reference format");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MinimumLength(2).WithMessage("Last name must be at least 2 characters");
        }
    }
}
