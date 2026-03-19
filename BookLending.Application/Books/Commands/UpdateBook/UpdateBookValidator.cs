using BookLending.Application.UnitOfWorkContract;
using BookLending.Domain.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace BookLending.Application.Books.Commands.UpdateBook
{
    public class UpdateBookValidator : AbstractValidator<UpdateBookCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateBookValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(x => x.UpdateBookDto.Id)
                .GreaterThan(0).WithMessage("Invalid book ID.");

            RuleFor(x => x.UpdateBookDto.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters.")
                .MustAsync(async (command, title, ct) =>
                {
                    var exists = await _unitOfWork.Repository<Book>()
                        .GetFiltered(b => b.Title == title && b.Id != command.UpdateBookDto.Id, asTracking: false)
                        .AnyAsync(ct);
                    return !exists;
                }).WithMessage("A book with this title already exists.");

            RuleFor(x => x.UpdateBookDto.Author)
                .NotEmpty().WithMessage("Author is required.")
                .MaximumLength(100).WithMessage("Author must not exceed 100 characters.");

            RuleFor(x => x.UpdateBookDto.ISBN)
                .NotEmpty().WithMessage("ISBN is required.")
                .Matches(@"^\d{10}$|^\d{13}$")
                .WithMessage("ISBN must be exactly 10 or 13 digits. Example: 0306406152 or 9780306406157")
                .MustAsync(async (command, isbn, ct) =>
                {
                    var exists = await _unitOfWork.Repository<Book>()
                        .GetFiltered(b => b.ISBN == isbn && b.Id != command.UpdateBookDto.Id, asTracking: false)
                        .AnyAsync(ct);
                    return !exists;
                }).WithMessage("A book with this ISBN already exists.");

            RuleFor(x => x.UpdateBookDto.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
                .When(x => x.UpdateBookDto.Description != null);
        }
    }
}