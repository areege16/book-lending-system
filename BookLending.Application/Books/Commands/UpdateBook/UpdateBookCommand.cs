using BookLending.Application.Common.Responses;
using BookLending.Application.DTOs.Book;
using MediatR;

namespace BookLending.Application.Books.Commands.UpdateBook
{
    public record UpdateBookCommand(UpdateBookDto UpdateBookDto) : IRequest<ResponseDto<bool>>;
}
