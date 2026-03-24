using FluentValidation;

namespace BookLending.Application.Account.Login
{
    class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.LoginRequestDto.Email)
                .NotEmpty()
                .WithMessage("Email is required");

            RuleFor(x => x.LoginRequestDto.Password)
                .NotEmpty()
                .WithMessage("Password is required");
        }
    }
}