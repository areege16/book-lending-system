using BookLending.Application.Common.Responses;
using BookLending.Application.UnitOfWorkContract;
using BookLending.Domain.Enums;
using BookLending.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLending.Application.Borrowing.Commands.BorrowBook
{
    public class BorrowBookHandler : IRequestHandler<BorrowBookCommand, ResponseDto<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BorrowBookHandler> _logger;

        public BorrowBookHandler(IUnitOfWork unitOfWork, ILogger<BorrowBookHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<ResponseDto<bool>> Handle(BorrowBookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User {UserId} attempting to borrow book {BookId}", request.UserId, request.BookId);

            var book = await _unitOfWork.Repository<Book>()
               .GetFiltered(b => b.Id == request.BookId, asTracking: true)
               .FirstOrDefaultAsync(cancellationToken);

            if (book == null)
            {
                _logger.LogWarning("Book not found with ID: {BookId}", request.BookId);
                return ResponseDto<bool>.Error(ErrorType.NotFound, "Book not found.");
            }

            if (!book.IsAvailable)
            {
                _logger.LogWarning("Book {BookId} is not available for borrowing.", request.BookId);
                return ResponseDto<bool>.Error(ErrorType.Conflict, "Book is not available for borrowing.");
            }

            var hasActiveBorrow = await _unitOfWork.Repository<BorrowingRecord>()
               .GetFiltered(br => br.UserId == request.UserId && br.ReturnDate == null, asTracking: false)
               .AnyAsync(cancellationToken);

            if (hasActiveBorrow)
            {
                _logger.LogWarning("User {UserId} already has an active borrowing.", request.UserId);
                return ResponseDto<bool>.Error(ErrorType.BadRequest, "You already have a borrowed book. Please return it first.");
            }

            var borrowingRecord = new BorrowingRecord
            {
                UserId = request.UserId,
                BookId = request.BookId,
                BorrowDate = DateTimeOffset.UtcNow,
                DueDate = DateTimeOffset.UtcNow.AddDays(7)
            };

            book.IsAvailable = false;

            _unitOfWork.Repository<BorrowingRecord>().Add(borrowingRecord);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {UserId} borrowed book {BookId} successfully. Due date: {DueDate}",
                request.UserId, request.BookId, borrowingRecord.DueDate);

            return ResponseDto<bool>.Success(true, $"Book borrowed successfully. Please return it by {borrowingRecord.DueDate:yyyy-MM-dd}.");
        }
    }
}