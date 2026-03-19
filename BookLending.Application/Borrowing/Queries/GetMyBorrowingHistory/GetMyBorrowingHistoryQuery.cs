using BookLending.Application.Common.Responses;
using BookLending.Application.DTOs.Borrowing;
using MediatR;

namespace BookLending.Application.Borrowing.Queries.GetMyBorrowingHistory
{
    public record GetMyBorrowingHistoryQuery(string UserId)
        : IRequest<ResponseDto<List<BorrowingRecordDto>>>;
}