using BookLending.Application.UnitOfWorkContract;
using BookLending.Domain.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BookLending.Application.Books.Commands.CreateBook
{
    public class CreateBookValidator : AbstractValidator<CreateBookCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateBookValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.CreateBookDto.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .MustAsync(async (title, ct) =>
                {
                    var exists = await _unitOfWork.Repository<Book>()
                        .GetFiltered(b => b.Title == title, asTracking: false)
                        .AnyAsync(ct);
                    return !exists;
                }).WithMessage("A book with this title already exists.");

            RuleFor(x => x.CreateBookDto.Author)
                .NotEmpty().WithMessage("Author is required.")
                .MaximumLength(100).WithMessage("Author must not exceed 100 characters.");

            RuleFor(x => x.CreateBookDto.ISBN)
                .NotEmpty().WithMessage("ISBN is required.")
                .Matches(@"^\d{10}$|^\d{13}$")
                .WithMessage("ISBN must be exactly 10 or 13 digits. Example: 0306406152 or 9780306406157")
                .MustAsync(async (isbn, ct) =>
                {
                    var exists = await _unitOfWork.Repository<Book>()
                        .GetFiltered(b => b.ISBN == isbn, asTracking: false)
                        .AnyAsync(ct);
                    return !exists;
                }).WithMessage("A book with this ISBN already exists.");

            RuleFor(x => x.CreateBookDto.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
                .When(x => x.CreateBookDto.Description != null);
        }
    }
}