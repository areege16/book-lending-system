using BookLending.Application.Common.Responses;
using BookLending.Application.UnitOfWorkContract;
using BookLending.Domain.Enums;
using BookLending.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLending.Application.Borrowing.Commands.ReturnBook
{
    public class ReturnBookHandler : IRequestHandler<ReturnBookCommand, ResponseDto<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReturnBookHandler> _logger;

        public ReturnBookHandler(IUnitOfWork unitOfWork, ILogger<ReturnBookHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ResponseDto<bool>> Handle(ReturnBookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User {UserId} attempting to return book {BookId}", request.UserId, request.BookId);

            var borrowingRecord = await _unitOfWork.Repository<BorrowingRecord>()
                .GetFiltered(br => br.UserId == request.UserId
                                && br.BookId == request.BookId
                                && br.ReturnDate == null, asTracking: true)
                .FirstOrDefaultAsync(cancellationToken);

            if (borrowingRecord == null)
            {
                _logger.LogWarning("No active borrowing found for User {UserId} and Book {BookId}",
                    request.UserId, request.BookId);
                return ResponseDto<bool>.Error(ErrorType.NotFound, "No active borrowing record found for this book.");
            }

            var book = await _unitOfWork.Repository<Book>()
                .GetFiltered(b => b.Id == request.BookId, asTracking: true)
                .FirstOrDefaultAsync(cancellationToken);

            borrowingRecord.ReturnDate = DateTimeOffset.UtcNow;
            book!.IsAvailable = true;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserId} returned book {BookId} successfully.", request.UserId, request.BookId);

            return ResponseDto<bool>.Success(true, "Book returned successfully.");
        }
    }
}