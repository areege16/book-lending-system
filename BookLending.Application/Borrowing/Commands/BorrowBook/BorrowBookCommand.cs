using BookLending.Application.Common.Responses;
using MediatR;

namespace BookLending.Application.Borrowing.Commands.BorrowBook
{
    public record BorrowBookCommand(int BookId, string UserId) : IRequest<ResponseDto<bool>>;
}