using AutoMapper;
using BookLending.Application.Common.Responses;
using BookLending.Application.UnitOfWorkContract;
using BookLending.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BookLending.Application.Books.Commands.CreateBook
{
    public class CreateBookHandler : IRequestHandler<CreateBookCommand, ResponseDto<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateBookHandler> _logger;
        private readonly IMapper _mapper;

        public CreateBookHandler(IUnitOfWork unitOfWork,
                                 ILogger<CreateBookHandler> logger,
                                 IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<ResponseDto<bool>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var createBookRequest = request.CreateBookDto;
            _logger.LogInformation("Attempting to create book: {Title}", createBookRequest.Title);

            var book = _mapper.Map<Book>(createBookRequest);
            _unitOfWork.Repository<Book>().Add(book);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Book created successfully with ID: {BookId}", book.Id);
            return ResponseDto<bool>.Success(true, "Book Created successfully");
        }
    }
}