using AutoMapper;
using BookLending.Application.Abstractions;
using BookLending.Application.Common.Responses;
using BookLending.Application.UnitOfWorkContract;
using BookLending.Domain.Enums;
using BookLending.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookLending.Application.Books.Commands.UpdateBook
{
    public class UpdateBookHandler : IRequestHandler<UpdateBookCommand, ResponseDto<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateBookHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public UpdateBookHandler(IUnitOfWork unitOfWork,
                                 ILogger<UpdateBookHandler> logger,
                                 IMapper mapper,
                                 IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<ResponseDto<bool>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var updateRequest = request.UpdateBookDto;

            _logger.LogInformation("Attempting to update book with ID: {BookId}", updateRequest.Id);

            var book = await _unitOfWork.Repository<Book>()
                                        .GetByIdAsync(updateRequest.Id);

            if (book == null)
            {
                _logger.LogWarning("Book not found with ID: {BookId}", updateRequest.Id);
                return ResponseDto<bool>.Error(ErrorType.NotFound, "Book not found.");
            }

            if (!string.IsNullOrWhiteSpace(updateRequest.CoverImagePath))
            {
                if (!string.IsNullOrEmpty(book.CoverImage))
                {
                    try
                    {
                        await _fileService.DeleteImageAsync(book.CoverImage);
                        _logger.LogInformation("Old cover image deleted for Book: {BookId}", updateRequest.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to delete old cover image for Book: {BookId}", updateRequest.Id);
                    }
                }
                book.CoverImage = updateRequest.CoverImagePath;
            }

            _mapper.Map(updateRequest, book);
            book.UpdatedAt = DateTimeOffset.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Book updated successfully with ID: {BookId}", book.Id);

            return ResponseDto<bool>.Success(true, "Book updated successfully.");
        }
    }
}