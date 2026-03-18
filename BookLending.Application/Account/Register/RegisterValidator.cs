using FluentValidation;

namespace BookLending.Application.Account.Register
{
    public class RegisterValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.RegisterRequestDto.UserName)
               .NotEmpty()
               .WithMessage("Username is required.");

            RuleFor(x => x.RegisterRequestDto.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithMessage("Please enter a valid email address.");

            RuleFor(x => x.RegisterRequestDto.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long.");

            RuleFor(x => x.RegisterRequestDto.ConfirmPassword)
                .NotEmpty()
                .WithMessage("Confirm password is required.")
                .Equal(x => x.RegisterRequestDto.Password)
                .WithMessage("Passwords do not match.");

            RuleFor(x => x.RegisterRequestDto.PhoneNumber)
                .NotEmpty()
                .WithMessage("Phone number is required.")
                .Matches(@"^(01[0125])[0-9]{8}$")
                .WithMessage("Invalid phone number format. Please enter a valid Egyptian number (e.g., 01012345678).");
        }
    }
}