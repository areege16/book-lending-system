using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookLending.Application.Common.Responses;
using BookLending.Application.DTOs.Book;
using BookLending.Application.UnitOfWorkContract;
using BookLending.Domain.Enums;
using BookLending.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLending.Application.Books.Queries.GetAllBooks
{
    public class GetAllBooksHandler : IRequestHandler<GetAllBooksQuery, ResponseDto<PagedResult<BookSummaryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllBooksHandler> _logger;

        public GetAllBooksHandler(IUnitOfWork unitOfWork,
                                  IMapper mapper,
                                  ILogger<GetAllBooksHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ResponseDto<PagedResult<BookSummaryDto>>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching all books.");

            var query = _unitOfWork.Repository<Book>().GetAllAsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                _logger.LogWarning("No books found.");
                return ResponseDto<PagedResult<BookSummaryDto>>.Error(ErrorType.NotFound, "No books found");
            }

            var books = await query
                .OrderBy(b => b.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ProjectTo<BookSummaryDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResult<BookSummaryDto>
            {
                Items = books,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };

            _logger.LogInformation("Fetched {Count} of {Total} books.", books.Count, totalCount);

            return ResponseDto<PagedResult<BookSummaryDto>>.Success(pagedResult, "Books fetched successfully.");
        }
    }
}