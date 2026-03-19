using BookLending.Application.Account.Register;
using BookLending.Application.DTOs.Account;
using BookLending.Domain.Models;
using BookLending.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace BookLending.Tests.Auth
{
    public class AuthServiceTests
    {
        private ApplicationContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationContext(options);
        }

        private UserManager<ApplicationUser> CreateUserManager(ApplicationContext context)
        {
            var store = new UserStore<ApplicationUser, ApplicationRole, ApplicationContext, string>(context);

            var options = Options.Create(new IdentityOptions());

            return new UserManager<ApplicationUser>(
                store,
                options,
                new PasswordHasher<ApplicationUser>(),
                new IUserValidator<ApplicationUser>[] { new UserValidator<ApplicationUser>() },
                new IPasswordValidator<ApplicationUser>[] { new PasswordValidator<ApplicationUser>() },
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null,
                Substitute.For<ILogger<UserManager<ApplicationUser>>>()
            );
        }

        private RoleManager<ApplicationRole> CreateRoleManager(ApplicationContext context)
        {
            var store = new RoleStore<ApplicationRole, ApplicationContext, string>(context);
            return new RoleManager<ApplicationRole>(
                store,
                null,
                new UpperInvariantLookupNormalizer(),
                null,
                Substitute.For<ILogger<RoleManager<ApplicationRole>>>()
            );
        }

        [Fact]
        public async Task Register_WhenValidData_ReturnsSuccess()
        {
            var context = CreateInMemoryContext();
            var userManager = CreateUserManager(context);
            var roleManager = CreateRoleManager(context);
            var logger = Substitute.For<ILogger<RegisterHandler>>();

            await roleManager.CreateAsync(new ApplicationRole { Name = "Reader" });

            var handler = new RegisterHandler(userManager, logger);

            var result = await handler.Handle(
                new RegisterCommand(
                    new RegisterRequestDto(
                        UserName: "testuser",
                        Name: "Test User",
                        Password: "Test@1234",
                        ConfirmPassword: "Test@1234",
                        Email: "test@test.com",
                        PhoneNumber: "01234567890"
                    )
                ), CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Register_WhenUserNameAlreadyExists_ReturnsError()
        {
            var context = CreateInMemoryContext();
            var userManager = CreateUserManager(context);
            var roleManager = CreateRoleManager(context);
            var logger = Substitute.For<ILogger<RegisterHandler>>();

            await roleManager.CreateAsync(new ApplicationRole { Name = "Reader" });

            var handler = new RegisterHandler(userManager, logger);

            var command = new RegisterCommand(
                new RegisterRequestDto(
                    UserName: "testuser",
                    Name: "Test User",
                    Password: "Test@1234",
                    ConfirmPassword: "Test@1234",
                    Email: "test@test.com",
                    PhoneNumber: "01234567890"
                ));

            await handler.Handle(command, CancellationToken.None);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess, "Expected failure for duplicate username");
        }

        [Fact]
        public async Task Register_WhenPasswordIsWeak_ReturnsError()
        {
            var context = CreateInMemoryContext();
            var userManager = CreateUserManager(context);
            var roleManager = CreateRoleManager(context);
            var logger = Substitute.For<ILogger<RegisterHandler>>();

            await roleManager.CreateAsync(new ApplicationRole { Name = "Reader" });
            var handler = new RegisterHandler(userManager, logger);

            var command = new RegisterCommand(
                new RegisterRequestDto(
                    UserName: "weakuser",
                    Name: "Weak User",
                    Password: "123",
                    ConfirmPassword: "123",
                    Email: "weak@test.com",
                    PhoneNumber: "01234567890"
                ));

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess, "Expected failure for weak password");
        }
    }
}
