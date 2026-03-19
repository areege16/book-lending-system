using BookLending.Application.Common.Responses;
using BookLending.Application.DTOs.Book;
using MediatR;

namespace BookLending.Application.Books.Queries.GetBookById
{
    public record GetBookByIdQuery(int Id) : IRequest<ResponseDto<BookDetailsDto>>;
}