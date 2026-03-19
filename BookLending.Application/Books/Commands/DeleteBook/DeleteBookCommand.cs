using BookLending.Application.Common.Responses;
using MediatR;

namespace BookLending.Application.Books.Commands.DeleteBook
{
    public record DeleteBookCommand(int Id) : IRequest<ResponseDto<bool>>;
}