using EndPointCommerce.AdminPortal.Services;
using EndPointCommerce.Domain.Entities;
using EndPointCommerce.IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Identity;

namespace EndPointCommerce.IntegrationTests.AdminPortal.Services;

public class UserSearcherTests : BaseEntitySearcherTests<UserSearchResultItem, UserWithRole>
{
    public UserSearcherTests(DatabaseFixture database) : base(database) { }

    protected override BaseEntitySearcher<UserSearchResultItem, UserWithRole> CreateSubject() =>
        new UserSearcher(_dbContext);

    protected override void PopulateRecords()
    {
        _dbContext.Users.Remove(_dbContext.Users.First());
        _dbContext.SaveChanges();

        _dbContext.Users.Add(new User { Id = 10, Email = "test_01@email.com" });
        _dbContext.UserRoles.Add(new IdentityUserRole<int> { UserId = 10, RoleId = 1 });

        _dbContext.Users.Add(new User { Id = 20, Email = "test_02@email.com" });
        _dbContext.UserRoles.Add(new IdentityUserRole<int> { UserId = 20, RoleId = 2 });

        _dbContext.Users.Add(new User { Id = 30, Email = "test_03@email.com" });
        _dbContext.UserRoles.Add(new IdentityUserRole<int> { UserId = 30, RoleId = 2 });

        _dbContext.Users.Add(new User { Id = 40, Email = "test_04@email.com" });
        _dbContext.UserRoles.Add(new IdentityUserRole<int> { UserId = 40, RoleId = 2 });

        _dbContext.Users.Add(new User { Id = 50, Email = "test_05@email.com" });
        _dbContext.UserRoles.Add(new IdentityUserRole<int> { UserId = 50, RoleId = 2 });

        _dbContext.SaveChanges();
    }

    [Theory]
    [InlineData("test_01@email.com", 10)]
    [InlineData("test_04@email.com", 40)]
    [InlineData("admin", 10)]
    public async Task Search_CanSearch(string searchValue, int matchedItemId)
    {
        await RunSearchingTheory(
            searchValue,
            result => {
                Assert.Equal(matchedItemId, result.Data.First().Id);
            }
        );
    }

    [Theory]
    [InlineData("email", "asc", 10)]
    [InlineData("email", "desc", 50)]
    [InlineData("role", "asc", 10)]
    public async Task Search_CanSort(string orderByName, string orderByDirection, int firstItemId)
    {
        await RunSortingTheory(
            orderByName, orderByDirection,
            result => {
                Assert.Equal(firstItemId, result.Data.First().Id);
            }
        );
    }

    [Fact]
    public async Task Search_ReturnsCorrectlyPopulatedData()
    {
        await _database.WithTransaction(_dbContext, async () =>
        {
            // Arrange
            PopulateRecords();

            var parameters = new SearchParameters
            {
                Draw = 1,
                Start = 0,
                Length = 100,
                Search = null,
                Order = null,
            };

            // Act
            var result = await _subject.Search(parameters, _mockUrlBuilder.Object);

            // Assert
            Assert.Equal(1, result.Draw);
            Assert.Equal(5, result.Data.Count);
            Assert.Contains(result.Data, item => item.Id == 10);
            Assert.Contains(result.Data, item => item.Email == "test_01@email.com");
            Assert.Contains(result.Data, item => item.Role == "Admin");
            Assert.Contains(result.Data, item => item.EditUrl == "test_edit_url");
        });
    }
}
