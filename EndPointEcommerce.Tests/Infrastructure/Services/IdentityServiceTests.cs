// Copyright 2025 End Point Corporation. Apache License, version 2.0.

using EndPointEcommerce.AdminPortal;
using EndPointEcommerce.Domain.Entities;
using EndPointEcommerce.Domain.Interfaces;
using EndPointEcommerce.Infrastructure.Services;
using EndPointEcommerce.Tests.Fixtures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace EndPointEcommerce.Tests.Infrastructure.Services
{
    public class IdentityServiceTests : TransactionalTests, IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;
        private readonly IIdentityService _subject;

        public IdentityServiceTests(DatabaseFixture database, WebApplicationFactory<Program> factory) : base(database)
        {
            factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(_ => dbContext);
                });
            });

            var scope = factory.Services.CreateScope();

            _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            _httpContextAccessor = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            _userClaimsPrincipalFactory = scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<User>>();
            _subject = scope.ServiceProvider.GetRequiredService<IIdentityService>();
        }

        [Fact]
        public async Task Exists_ReturnsFalse_WhenTheUserDoesNotExist()
        {
            // Act
            var result = await _subject.Exists(123);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Exists_ReturnsTrue_WhenTheUserExists()
        {
            // Arrange
            dbContext.Users.Add(new() { Id = 123 });
            await dbContext.SaveChangesAsync();

            // Act
            var result = await _subject.Exists(123);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task FindByIdAsync_ReturnsNull_WhenTheUserDoesNotExist()
        {
            // Act
            var result = await _subject.FindByIdAsync(123);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindByIdAsync_ReturnsTheUser_WhenTheUserExists()
        {
            // Arrange
            dbContext.Users.Add(new()
            {
                Id = 123,
                Customer = new()
                {
                    Name = "test_customer_name",
                    Email = "test_customer@email.com"
                }
            });

            await dbContext.SaveChangesAsync();

            // Act
            var result = await _subject.FindByIdAsync(123);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Customer);
        }

        [Fact]
        public async Task FindByUserNameAsync_ReturnsNull_WhenTheUserDoesNotExist()
        {
            // Act
            var result = await _subject.FindByUserNameAsync("test_user_name");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task FindByUserNameAsync_ReturnsTheUser_WhenTheUserExists()
        {
            // Arrange
            dbContext.Users.Add(new()
            {
                Id = 123,
                UserName = "test_user_name",
                Customer = new()
                {
                    Name = "test_customer_name",
                    Email = "test_customer@email.com"
                }
            });

            await dbContext.SaveChangesAsync();

            // Act
            var result = await _subject.FindByUserNameAsync("test_user_name");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Customer);
        }

        [Fact]
        public async Task FindByUserNameAsync_ReturnsTheUser_WhenTheUserExists_CaseInsensitively()
        {
            // Arrange
            dbContext.Users.Add(new()
            {
                Id = 123,
                UserName = "test_user_name",
                Customer = new()
                {
                    Name = "test_customer_name",
                    Email = "test_customer@email.com"
                }
            });

            await dbContext.SaveChangesAsync();

            // Act
            var result = await _subject.FindByUserNameAsync("TEST_USER_NAME");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Customer);
        }

        [Fact]
        public async Task AddAsync_CreatesANewUserOfTypeCustomer()
        {
            // Arrange
            var user = new User
            {
                UserName = "test_user_name",
                Email = "test_user@email.com",
            };

            // Act
            var result = await _subject.AddAsync(user, "Test_Password_123", User.CUSTOMER_ROLE);

            // Assert
            Assert.True(result.Succeeded);

            user = await dbContext.Users.SingleOrDefaultAsync(u => u.UserName == "test_user_name");

            Assert.NotNull(user);
            Assert.Equal("test_user@email.com", user.Email);
            Assert.True(await _userManager.IsInRoleAsync(user, User.CUSTOMER_ROLE));
        }

        [Fact]
        public async Task AddAsync_CreatesANewUserOfTypeAdmin()
        {
            // Arrange
            var user = new User
            {
                UserName = "test_user_name",
                Email = "test_user@email.com",
            };

            // Act
            var result = await _subject.AddAsync(user, "Test_Password_123", User.ADMIN_ROLE);

            // Assert
            Assert.True(result.Succeeded);

            user = await dbContext.Users.SingleOrDefaultAsync(u => u.UserName == "test_user_name");

            Assert.NotNull(user);
            Assert.Equal("test_user@email.com", user.Email);
            Assert.True(await _userManager.IsInRoleAsync(user, User.ADMIN_ROLE));
        }

        [Fact]
        public async Task AddAsync_DoesNotCreateANewUser_WhenThePasswordIsNotStrong()
        {
            // Arrange
            var user = new User
            {
                UserName = "test_user_name",
                Email = "test_user@email.com"
            };

            // Act
            var result = await _subject.AddAsync(user, "weak", "asdasd");

            // Assert
            Assert.False(result.Succeeded);
            Assert.Contains(result.Errors, e => e.Code == "PasswordTooShort");
            Assert.Contains(result.Errors, e => e.Code == "PasswordRequiresDigit");
            Assert.Contains(result.Errors, e => e.Code == "PasswordRequiresUpper");

            user = await dbContext.Users.SingleOrDefaultAsync(u => u.UserName == "test_user_name");
            Assert.Null(user);
        }

        [Fact]
        public async Task UpdateAsync_ChangesTheRole()
        {
            // Arrange
            var user = new User
            {
                UserName = "test_user_name",
                Email = "test_user@email.com",
            };

            var result = await _subject.AddAsync(user, "Test_Password_123", User.CUSTOMER_ROLE);

            Assert.True(result.Succeeded);

            user = dbContext.Users.Single(u => u.UserName == "test_user_name");

            // Act
            result = await _subject.UpdateAsync(user, null, User.ADMIN_ROLE);

            // Assert
            Assert.True(result.Succeeded);

            user = dbContext.Users.Single(u => u.UserName == "test_user_name");

            Assert.True(await _userManager.IsInRoleAsync(user, User.ADMIN_ROLE));
        }

        [Fact]
        public async Task UpdateAsync_ChangesTheEmail()
        {
            // Arrange
            var user = new User
            {
                UserName = "test_user_name",
                Email = "test_user@email.com",
            };

            var result = await _subject.AddAsync(user, "Test_Password_123", User.CUSTOMER_ROLE);

            Assert.True(result.Succeeded);

            user = dbContext.Users.Single(u => u.UserName == "test_user_name");

            user.Email = "another_test@email.com";

            // Act
            result = await _subject.UpdateAsync(user, null, null);

            // Assert
            Assert.True(result.Succeeded);

            user = dbContext.Users.Single(u => u.UserName == "test_user_name");

            Assert.Equal("another_test@email.com", user.Email);
        }

        [Fact]
        public async Task UpdateAsync_ChangesTheUsername()
        {
            // Arrange
            var user = new User
            {
                UserName = "test_user_name",
                Email = "test_user@email.com",
            };

            var result = await _subject.AddAsync(user, "Test_Password_123", User.CUSTOMER_ROLE);

            Assert.True(result.Succeeded);

            user = dbContext.Users.Single(u => u.UserName == "test_user_name");

            user.UserName = "another_test_user_name";

            // Act
            result = await _subject.UpdateAsync(user, null, null);

            // Assert
            Assert.True(result.Succeeded);

            user = dbContext.Users.SingleOrDefault(u => u.UserName == "another_test_user_name");

            Assert.NotNull(user);
        }

        [Fact]
        public async Task UpdateAsync_ChangesThePassword()
        {
            // Arrange
            var user = new User
            {
                UserName = "test_user_name",
                Email = "test_user@email.com",
            };

            var result = await _subject.AddAsync(user, "Test_Password_123", User.CUSTOMER_ROLE);

            Assert.True(result.Succeeded);

            user = dbContext.Users.Single(u => u.UserName == "test_user_name");

            // Act
            result = await _subject.UpdateAsync(user, "New_Test_Password_123", null);

            // Assert
            Assert.True(result.Succeeded);

            user = dbContext.Users.Single(u => u.UserName == "test_user_name");
            Assert.True(await _userManager.CheckPasswordAsync(user, "New_Test_Password_123"));
        }

        [Fact]
        public async Task DeleteAsync_DeletesTheUser_WhenItExists()
        {
            // Arrange
            var user = new User
            {
                UserName = "test_user_name",
                Email = "test_user@email.com",
            };

            var result = await _subject.AddAsync(user, "Test_Password_123", User.CUSTOMER_ROLE);

            Assert.True(result.Succeeded);

            // Act
            result = await _subject.DeleteAsync(user);

            // Assert
            Assert.True(result.Succeeded);

            user = dbContext.Users.FirstOrDefault(u => u.UserName == "test_user_name");
            Assert.Null(user);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFailure_WhenTheUserDoesNotExist()
        {
            // Arrange
            var user = new User
            {
                Id = 123,
                UserName = "test_user_name",
                Email = "test_user@email.com",
            };

            // Act
            var result = await _subject.DeleteAsync(user);

            // Assert
            Assert.False(result.Succeeded);
        }

        [Fact]
        public async Task IsPasswordValid_ReturnsTrueIfThePasswordIsValid()
        {
            // Arrange
            var user = new User
            {
                UserName = "test_user_name",
                Email = "test_user@email.com",
            };

            var result = await _subject.AddAsync(user, "Test_Password_123", User.CUSTOMER_ROLE);

            Assert.True(result.Succeeded);

            // Assert
            Assert.True(await _subject.IsPasswordValid(user, "Test_Password_123"));
        }

        [Fact]
        public async Task IsPasswordValid_ReturnsFalseIfThePasswordIsNotValid()
        {
            // Arrange
            var user = new User
            {
                UserName = "test_user_name",
                Email = "test_user@email.com",
            };

            var result = await _subject.AddAsync(user, "Test_Password_123", User.CUSTOMER_ROLE);

            Assert.True(result.Succeeded);

            // Assert
            Assert.False(await _subject.IsPasswordValid(user, "a wrong password"));
        }

        [Fact]
        public async Task LoginAsync_ReturnsSuccess_IfTheSignInManagerCanSignInTheUser()
        {
            // Arrange
            var email = "test_user@email.com";
            var password = "Test_Password_123";

            var _mockSignInManager = new Mock<SignInManager<User>>(
                _userManager,
                _httpContextAccessor,
                _userClaimsPrincipalFactory,
                null!, null!, null!, null!
            );

            _mockSignInManager
                .Setup(s => s.PasswordSignInAsync(email, password, true, false))
                .ReturnsAsync(SignInResult.Success);

            var _subject = new IdentityService(
                null!, _mockSignInManager.Object, null!, null!
            );

            // Act
            var result = await _subject.LoginAsync(email, password);

            // Assert
            Assert.Equal(SignInResult.Success, result);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFailure_IfTheSignInManagerCanNotSignInTheUser()
        {
            // Arrange
            var email = "test_user@email.com";
            var password = "Test_Password_123";

            var _mockSignInManager = new Mock<SignInManager<User>>(
                _userManager,
                _httpContextAccessor,
                _userClaimsPrincipalFactory,
                null!, null!, null!, null!
            );

            _mockSignInManager
                .Setup(s => s.PasswordSignInAsync(email, password, true, false))
                .ReturnsAsync(SignInResult.Failed);

            var _subject = new IdentityService(
                null!, _mockSignInManager.Object, null!, null!
            );

            // Act
            var result = await _subject.LoginAsync(email, password);

            // Assert
            Assert.Equal(SignInResult.Failed, result);
        }

        [Fact]
        public async Task LogoutAsync_CallsOnTheSignInManagerToSignOutTheCurrentUser()
        {
            // Arrange
            var _mockSignInManager = new Mock<SignInManager<User>>(
                _userManager,
                _httpContextAccessor,
                _userClaimsPrincipalFactory,
                null!, null!, null!, null!
            );

            var _subject = new IdentityService(
                null!, _mockSignInManager.Object, null!, null!
            );

            // Act
            await _subject.LogoutAsync();

            // Assert
            _mockSignInManager.Verify(s => s.SignOutAsync(), Times.Once);
        }
    }
}
