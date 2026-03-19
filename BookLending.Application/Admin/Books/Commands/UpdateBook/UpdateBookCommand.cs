using BookLending.Application.Common.Responses;
using BookLending.Application.DTOs.Admin.Book;
using MediatR;

namespace BookLending.Application.Admin.Books.Commands.UpdateBook
{
    public record UpdateBookCommand(UpdateBookDto UpdateBookDto) : IRequest<ResponseDto<bool>>;
}
