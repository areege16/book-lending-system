using BookLending.Api.Extensions;
using BookLending.Application.Borrowing.Commands.BorrowBook;
using BookLending.Application.Borrowing.Commands.ReturnBook;
using BookLending.Application.Borrowing.Queries.GetAllBorrowings;
using BookLending.Application.Borrowing.Queries.GetMyBorrowingHistory;
using BookLending.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLending.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class BorrowingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BorrowingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region BorrowBook
        [HttpPost("reader/borrow/{bookId}")]
        [Authorize(Roles = Roles.Reader)]
        public async Task<IActionResult> BorrowBook(int bookId)
        {
            var userId = User.GetUserId();
            var result = await _mediator.Send(new BorrowBookCommand(bookId, userId));
            return Ok(result);
        }
        #endregion

        #region ReturnBook
        [HttpPost("reader/return/{bookId}")]
        [Authorize(Roles = Roles.Reader)]
        public async Task<IActionResult> ReturnBook(int bookId)
        {
            var userId = User.GetUserId();
            var result = await _mediator.Send(new ReturnBookCommand(bookId, userId));
            return Ok(result);
        }
        #endregion

        #region GetBorrowingHistory
        [HttpGet("reader/my-history")]
        [Authorize(Roles = Roles.Reader)]
        public async Task<IActionResult> GetMyBorrowingHistory()
        {
            var userId = User.GetUserId();
            var result = await _mediator.Send(new GetMyBorrowingHistoryQuery(userId));
            return Ok(result);
        }
        #endregion
        [HttpGet("admin/records")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> GetAllBorrowings()
        {
            var result = await _mediator.Send(new GetAllBorrowingsQuery());
            return Ok(result);
        }
    }
}