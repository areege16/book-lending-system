using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookLending.Application.Common.Responses;
using BookLending.Application.DTOs.Borrowing;
using BookLending.Application.UnitOfWorkContract;
using BookLending.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLending.Application.Borrowing.Queries.GetAllBorrowings
{
    public class GetAllBorrowingsHandler : IRequestHandler<GetAllBorrowingsQuery, ResponseDto<List<AdminBorrowingRecordDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllBorrowingsHandler> _logger;

        public GetAllBorrowingsHandler(IUnitOfWork unitOfWork,
                                       IMapper mapper,
                                       ILogger<GetAllBorrowingsHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ResponseDto<List<AdminBorrowingRecordDto>>> Handle(GetAllBorrowingsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Admin fetching all borrowing records.");

            var records = await _unitOfWork.Repository<BorrowingRecord>()
                .GetAllAsNoTracking()
                .Include(br => br.Book)
                .Include(br => br.User)
                .OrderBy(br => br.BorrowDate)
                .ProjectTo<AdminBorrowingRecordDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Fetched {Count} borrowing records.", records.Count);
            return ResponseDto<List<AdminBorrowingRecordDto>>.Success(records, "Borrowing records fetched successfully.");
        }
    }
}