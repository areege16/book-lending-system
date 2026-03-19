using BookLending.Application.Common.Responses;
using BookLending.Application.DTOs.Borrowing;
using MediatR;

namespace BookLending.Application.Borrowing.Queries.GetAllBorrowings
{
    public record GetAllBorrowingsQuery() : IRequest<ResponseDto<List<AdminBorrowingRecordDto>>>;
}