using AutoMapper;
using BookLending.Application.Books.Commands.CreateBook;
using BookLending.Application.DTOs.Book;
using BookLending.Domain.Models;
using BookLending.Infrastructure.Context;
using BookLending.Infrastructure.UnitOfWorkImplementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace BookLending.Tests.Books
{
    public class BookServiceTests
    {
        private ApplicationContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationContext(options);
        }

        [Fact]
        public async Task CreateBook_WhenBookIsValid_ReturnsSuccess()
        {
            var context = CreateInMemoryContext();
            var unitOfWork = new UnitOfWork(context);
            var logger = Substitute.For<ILogger<CreateBookHandler>>();
            var mapper = Substitute.For<IMapper>();

            mapper.Map<Book>(Arg.Any<CreateBookDto>()).Returns(new Book
            {
                Title = "Clean Code",
                Author = "Robert Martin",
                ISBN = "978-0132350884",
                Description = "A book about clean code"
            });

            var handler = new CreateBookHandler(unitOfWork, logger, mapper);

            var result = await handler.Handle(
                new CreateBookCommand(
                    new CreateBookDto("Clean Code", "Robert Martin", "978-0132350884", "A book about clean code", null)
                ), CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

    }
}