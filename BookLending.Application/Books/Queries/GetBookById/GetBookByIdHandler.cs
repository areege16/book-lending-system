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

namespace BookLending.Application.Books.Queries.GetBookById
{
    public class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, ResponseDto<BookDetailsDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBookByIdHandler> _logger;

        public GetBookByIdHandler(IUnitOfWork unitOfWork,
                                  IMapper mapper,
                                  ILogger<GetBookByIdHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ResponseDto<BookDetailsDto>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching book with ID: {BookId}", request.Id);

            var book = await _unitOfWork.Repository<Book>()
                .GetFiltered(b => b.Id == request.Id, asTracking: false)
                .ProjectTo<BookDetailsDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (book == null)
            {
                _logger.LogWarning("Book not found with ID: {BookId}", request.Id);
                return ResponseDto<BookDetailsDto>.Error(ErrorType.NotFound, "Book not found.");
            }

            _logger.LogInformation("Book fetched successfully with ID: {BookId}", request.Id);
            return ResponseDto<BookDetailsDto>.Success(book, "Book fetched successfully.");
        }
    }
}