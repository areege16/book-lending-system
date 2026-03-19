using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookLending.Application.Common.Responses;
using BookLending.Application.DTOs.Borrowing;
using BookLending.Application.UnitOfWorkContract;
using BookLending.Domain.Enums;
using BookLending.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLending.Application.Borrowing.Queries.GetMyBorrowingHistory
{
    public class GetMyBorrowingHistoryHandler : IRequestHandler<GetMyBorrowingHistoryQuery, ResponseDto<List<BorrowingRecordDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetMyBorrowingHistoryHandler> _logger;

        public GetMyBorrowingHistoryHandler(IUnitOfWork unitOfWork,
                                            IMapper mapper,
                                            ILogger<GetMyBorrowingHistoryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ResponseDto<List<BorrowingRecordDto>>> Handle(GetMyBorrowingHistoryQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching borrowing history for User {UserId}", request.UserId);

            var records = await _unitOfWork.Repository<BorrowingRecord>()
               .GetFiltered(br => br.UserId == request.UserId, asTracking: false)
               .Include(br => br.Book)
               .OrderBy(br => br.BorrowDate)
               .ProjectTo<BorrowingRecordDto>(_mapper.ConfigurationProvider)
               .ToListAsync(cancellationToken);

            if (records.Count == 0)
            {
                _logger.LogWarning("No borrowing history found for User {UserId}", request.UserId);
                return ResponseDto<List<BorrowingRecordDto>>.Error(ErrorType.NotFound, "No borrowing history found.");
            }

            _logger.LogInformation("Fetched {Count} records for User {UserId}", records.Count, request.UserId);
            return ResponseDto<List<BorrowingRecordDto>>.Success(records, "Borrowing history fetched successfully.");
        }
    }
}