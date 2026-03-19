using BookLending.Application.Common.Responses;
using MediatR;

namespace BookLending.Application.Borrowing.Commands.ReturnBook
{
    public record ReturnBookCommand(int BookId, string UserId) : IRequest<ResponseDto<bool>>;
}