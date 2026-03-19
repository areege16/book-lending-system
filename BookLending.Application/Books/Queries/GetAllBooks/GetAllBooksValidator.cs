using FluentValidation;

namespace BookLending.Application.Books.Queries.GetAllBooks
{
    public class GetAllBooksValidator : AbstractValidator<GetAllBooksQuery>
    {
        public GetAllBooksValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 50)
                .WithMessage("Page size must be between 1 and 50");
        }
    }
}