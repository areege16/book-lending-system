using FluentValidation;

namespace BookLending.Application.Borrowing.Commands.ReturnBook
{
    public class ReturnBookValidator : AbstractValidator<ReturnBookCommand>
    {
        public ReturnBookValidator()
        {
            RuleFor(x => x.BookId)
                .GreaterThan(0).WithMessage("Invalid book ID.");
        }
    }
}