using FluentValidation;

namespace BookLending.Application.Borrowing.Commands.BorrowBook
{
    public class BorrowBookValidator : AbstractValidator<BorrowBookCommand>
    {
        public BorrowBookValidator()
        {
            RuleFor(x => x.BookId)
                .GreaterThan(0).WithMessage("Invalid book ID.");
        }
    }
}