using BookLending.Application.Common.Responses;
using BookLending.Application.DTOs.Book;
using MediatR;

namespace BookLending.Application.Books.Queries.GetAllBooks
{
    public record GetAllBooksQuery(int PageNumber = 1, int PageSize = 10) : IRequest<ResponseDto<PagedResult<BookSummaryDto>>>;
}