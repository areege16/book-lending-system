using BookLending.Application.Abstractions;
using BookLending.Application.Common.Responses;
using BookLending.Application.UnitOfWorkContract;
using BookLending.Domain.Enums;
using BookLending.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookLending.Application.Books.Commands.DeleteBook
{
    public class DeleteBookHandler : IRequestHandler<DeleteBookCommand, ResponseDto<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeleteBookHandler> _logger;
        private readonly IFileService _fileService;

        public DeleteBookHandler(IUnitOfWork unitOfWork,
                                 ILogger<DeleteBookHandler> logger,
                                 IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _fileService = fileService;
        }

        public async Task<ResponseDto<bool>> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete book with ID: {BookId}", request.Id);

            var book = await _unitOfWork.Repository<Book>()
                                        .GetByIdAsync(request.Id);

            if (book == null)
            {
                _logger.LogWarning("Book not found with ID: {BookId}", request.Id);
                return ResponseDto<bool>.Error(ErrorType.NotFound, "Book not found.");
            }

            var hasBorrowingHistory = await _unitOfWork.Repository<BorrowingRecord>()
                                                       .GetFiltered(br => br.BookId == request.Id, asTracking: false)
                                                       .AnyAsync(cancellationToken);

            if (hasBorrowingHistory)
            {
                _logger.LogWarning("Cannot delete book {BookId}: it has borrowing history.", request.Id);
                return ResponseDto<bool>.Error(ErrorType.Conflict, "This book cannot be deleted because it has borrowing records.");
            }
            if (!string.IsNullOrEmpty(book.CoverImage))
            {
                try
                {
                    await _fileService.DeleteImageAsync(book.CoverImage);
                    _logger.LogInformation("Cover image deleted for Book: {BookId}", request.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to delete cover image for Book: {BookId}", request.Id);
                }
            }

            _unitOfWork.Repository<Book>().Remove(book);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Book deleted successfully with ID: {BookId}", request.Id);
            return ResponseDto<bool>.Success(true, "Book deleted successfully.");
        }
    }
}
