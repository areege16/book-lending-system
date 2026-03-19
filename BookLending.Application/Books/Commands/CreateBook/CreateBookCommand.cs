using BookLending.Application.Common.Responses;
using BookLending.Application.DTOs.Book;
using MediatR;

namespace BookLending.Application.Books.Commands.CreateBook
{
    public record CreateBookCommand(CreateBookDto CreateBookDto) : IRequest<ResponseDto<bool>>;
}