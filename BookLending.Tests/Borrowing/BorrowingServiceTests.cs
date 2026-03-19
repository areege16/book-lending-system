using BookLending.Application.Borrowing.Commands.BorrowBook;
using BookLending.Domain.Models;
using BookLending.Infrastructure.Context;
using BookLending.Infrastructure.UnitOfWorkImplementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BookLending.Tests.Borrowing
{
    public class BorrowingServiceTests
    {
        private ApplicationContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationContext(options);
        }

        [Fact]
        public async Task BorrowBook_WhenBookIsNotAvailable_ReturnsError()
        {
            var context = CreateInMemoryContext();
            var unitOfWork = new UnitOfWork(context);
            var logger = Substitute.For<ILogger<BorrowBookHandler>>();

            context.Books.Add(new Book
            {
                Id = 1,
                Title = "Clean Code",
                Author = "Robert Martin",
                ISBN = "978-0132350884",
                IsAvailable = false
            });
            
            await context.SaveChangesAsync();

            var handler = new BorrowBookHandler(unitOfWork, logger);

            var result = await handler.Handle(new BorrowBookCommand(1, "user-id"), CancellationToken.None);

            Assert.False(result.IsSuccess, "Expected failure because book is not available");
        }

        [Fact]
        public async Task BorrowBook_WhenBookIsAvailable_ReturnsSuccess()
        {
            var context = CreateInMemoryContext();
            var unitOfWork = new UnitOfWork(context);
            var logger = Substitute.For<ILogger<BorrowBookHandler>>();

            context.Books.Add(new Book
            {
                Id = 1,
                Title = "Clean Code",
                Author = "Robert Martin",
                ISBN = "978-0132350884",
                IsAvailable = true
            });

            await context.SaveChangesAsync();

            var handler = new BorrowBookHandler(unitOfWork, logger);

            var result = await handler.Handle(new BorrowBookCommand(1, "user-id"), CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task BorrowBook_WhenUserAlreadyHasActiveBorrow_ReturnsError()
        {
            var context = CreateInMemoryContext();
            var unitOfWork = new UnitOfWork(context);
            var logger = Substitute.For<ILogger<BorrowBookHandler>>();

            context.Books.Add(new Book
            {
                Id = 1,
                Title = "Clean Code",
                Author = "Robert Martin",
                ISBN = "978-0132350884",
                IsAvailable = true
            });

            context.BorrowingRecords.Add(new BorrowingRecord
            {
                UserId = "user-id",
                BookId = 1,
                BorrowDate = DateTimeOffset.UtcNow,
                DueDate = DateTimeOffset.UtcNow.AddDays(7),
                ReturnDate = null
            });

            await context.SaveChangesAsync();

            var handler = new BorrowBookHandler(unitOfWork, logger);

            var result = await handler.Handle(new BorrowBookCommand(1, "user-id"), CancellationToken.None);

            Assert.False(result.IsSuccess, "Expected failure because user already has an active borrow");
        }
    }
}