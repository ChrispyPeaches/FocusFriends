using System.Net;
using System.Linq.Expressions;
using FocusAPI.Methods.User;
using FocusAPI.Models;
using FocusAPI.Repositories;
using FocusAPI.Tests.Fakers;
using FocusCore.Queries.User;
using Moq;
using Shouldly;
using FocusCore.Models;

namespace FocusAPI.Tests.UserMethods;
public class GetUserTests
{
    BaseUserFaker _baseUserFaker;
    Mock<IUserRepository> _userRepository;
    public GetUserTests()
    {
        SetupTestHelpers();
        SetupSystemDependencies();
    }

    void SetupTestHelpers()
    {
        _baseUserFaker = new BaseUserFaker();
    }

    void SetupSystemDependencies()
    {
        _userRepository = new Mock<IUserRepository>();
    }

    void SetupMockDataset(List<BaseUser> users)
    {
        _userRepository.Setup(repo => repo.GetBaseUserWithItemsByAuth0IdAsync(
            It.IsAny<string?>(),
            It.IsAny<Expression<Func<User, bool>>[]?>(),
            It.IsAny<CancellationToken>()
            ))
            .ReturnsAsync(users.FirstOrDefault());
    }

    void SetupMockExceptionCallback(string exceptionMessage)
    {
        _userRepository.Setup(repo => repo.GetBaseUserWithItemsByAuth0IdAsync(
            It.IsAny<string?>(),
            It.IsAny<Expression<Func<User, bool>>[]?>(),
            It.IsAny<CancellationToken>()
            ))
            .Callback(() => throw new Exception(exceptionMessage));
    }

    [Fact]
    public async Task GetUser_ReturnsOKAndItemIds_WhenUserIsNotNull_AndUserHasItems()
    {
        // ARRANGE
        BaseUser user = _baseUserFaker.Generate();
        SetupMockDataset([user]);

        GetUser.Handler handler = new(_userRepository.Object);
        GetUserQuery query = new GetUserQuery { Auth0Id = user.Auth0Id };

        // ACT
        var result = await handler.Handle(query);

        // ASSERT
        result.HttpStatusCode.ShouldBe(HttpStatusCode.OK);
        result.Message.ShouldBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.UserBadgeIds.ShouldNotBeEmpty();
        result.Data.UserDecorIds.ShouldNotBeEmpty();
        result.Data.UserPetIds.ShouldNotBeEmpty();
        result.Data.UserIslandIds.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetUser_ReturnsOKAndNoItemIds_WhenUserIsNotNull_AndUserHasNoItems()
    {
        // ARRANGE
        // Set up base user with no items
        BaseUser user = _baseUserFaker.Generate();
        user.Badges = null;
        user.Decor = null;
        user.Islands = null;
        user.Pets = null;
        SetupMockDataset([user]);

        GetUser.Handler handler = new(_userRepository.Object);
        GetUserQuery query = new GetUserQuery { Auth0Id = user.Auth0Id };

        // ACT
        var result = await handler.Handle(query);

        // ASSERT
        result.HttpStatusCode.ShouldBe(HttpStatusCode.OK);
        result.Message.ShouldBeNull();
        result.Data.ShouldNotBeNull();
        result.Data.UserBadgeIds.ShouldBeEmpty();
        result.Data.UserDecorIds.ShouldBeEmpty();
        result.Data.UserPetIds.ShouldBeEmpty();
        result.Data.UserIslandIds.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetUser_ReturnsNotFound_WhenUserIsNull()
    {
        // ARRANGE
        // Set up mock to return no user
        SetupMockDataset([]);
        GetUser.Handler handler = new(_userRepository.Object);
        GetUserQuery query = new GetUserQuery { Auth0Id = "" };

        // ACT
        var result = await handler.Handle(query);

        // ASSERT
        result.HttpStatusCode.ShouldBe(HttpStatusCode.NotFound);
        result.Message.ShouldBe($"User not found with Auth0Id: {query.Auth0Id}");
        result.Data.ShouldBeNull();
    }

    [Fact]
    public async Task GetUser_ThrowsException_WhenErrorEncounteredDuringQuery()
    {
        // ARRANGE
        // Set up mock to throw exception
        string exceptionMessage = "Test exception message.";
        SetupMockExceptionCallback(exceptionMessage);
        GetUser.Handler handler = new(_userRepository.Object);
        GetUserQuery query = new GetUserQuery { Auth0Id = "" };

        // ACT / ASSERT
        await Should.ThrowAsync<Exception>(() => handler.Handle(query), $"Error getting user: {exceptionMessage}");
    }
}