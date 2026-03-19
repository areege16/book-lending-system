using BookLending.Api.Requests.Books;
using BookLending.Application.Abstractions;
using BookLending.Application.Admin.Books.Commands.CreateBook;
using BookLending.Application.Admin.Books.Commands.DeleteBook;
using BookLending.Application.Admin.Books.Commands.UpdateBook;
using BookLending.Application.DTOs.Admin.Book;
using BookLending.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLending.Api.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFileService _fileService;

        public BooksController(IMediator mediator, IFileService fileService)
        {
            _mediator = mediator;
            _fileService = fileService;
        }

        #region CreateBook
        [HttpPost]
        public async Task<IActionResult> CreateBook([FromForm] CreateBookRequest request)
        {
            string? coverImage = null;

            if (request.CoverImage != null && request.CoverImage.Length > 0)
            {
                coverImage = await _fileService.UploadImageAsync(request.CoverImage, "BookLending/covers");
            }

            var createBookDto = new CreateBookDto(
                Title: request.Title,
                Author: request.Author,
                ISBN: request.ISBN,
                Description: request.Description,
                CoverImage: coverImage
                );
            var result = await _mediator.Send(new CreateBookCommand
            (
                createBookDto
            ));
            return Ok(result);
        }
        #endregion

        #region UpdateBook
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromForm] UpdateBookRequest request)
        {
            string? coverImagePath = null;

            if (request.CoverImage != null && request.CoverImage.Length > 0)
            {
                coverImagePath = await _fileService.UploadImageAsync(request.CoverImage, "BookLending/covers");
            }

            var updateBookDto = new UpdateBookDto(
                Id: id,
                Title: request.Title,
                Author: request.Author,
                ISBN: request.ISBN,
                Description: request.Description,
                CoverImagePath: coverImagePath
            );

            var result = await _mediator.Send(new UpdateBookCommand(updateBookDto));
            return Ok(result);
        }
        #endregion

        #region DeleteBook
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var result = await _mediator.Send(new DeleteBookCommand(id));
            return Ok(result);
        }
        #endregion
    }
}