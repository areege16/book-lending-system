using BookLending.Application.Common.Responses;
using BookLending.Application.DTOs.Admin.Book;
using MediatR;

namespace BookLending.Application.Admin.Books.Commands.CreateBook
{
    public record CreateBookCommand(CreateBookDto CreateBookDto) : IRequest<ResponseDto<bool>>;
}