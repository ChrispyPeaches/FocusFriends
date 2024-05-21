using System.Net;
using System.Linq.Expressions;
using FocusAPI.Methods.User;
using FocusAPI.Models;
using FocusCore.Queries.User;
using Moq;
using Shouldly;
using FocusCore.Models;
using FocusAPI.Data;
using Moq.EntityFrameworkCore;
using FocusAPI.Tests.Fakers.ImplementedModels;

namespace FocusAPI.Tests.UserMethods;
public class GetUserTests
{
    UserFaker _userFaker;
    Mock<IFocusAPIContext> _context;
    public GetUserTests()
    {
        SetupTestHelpers();
        SetupSystemDependencies();
    }

    void SetupTestHelpers()
    {
        _userFaker = new UserFaker();
    }

    void SetupSystemDependencies()
    {
        _context = new Mock<IFocusAPIContext>();
    }

    void SetupMockDataset(IList<User> users)
    {
        _context.Setup(x => x.Users)
            .ReturnsDbSet(users);
    }

    void SetupMockExceptionCallback(string exceptionMessage)
    {
        _context.Setup(x => x.Users)
            .Callback(() => throw new Exception(exceptionMessage));
    }

    [Fact]
    public async Task GetUser_ReturnsOKAndItemIds_WhenUserIsNotNull_AndUserHasItems()
    {
        // ARRANGE
        User user = _userFaker.Generate();
        SetupMockDataset([user]);

        GetUser.Handler handler = new(_context.Object);
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
        User user = _userFaker.Generate();
        user.Badges = new List<UserBadge>();
        user.Decor = new List<UserDecor>();
        user.Islands = new List<UserIsland>();
        user.Pets = new List<UserPet>();
        SetupMockDataset([user]);

        GetUser.Handler handler = new(_context.Object);
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
        GetUser.Handler handler = new(_context.Object);
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
        GetUser.Handler handler = new(_context.Object);
        GetUserQuery query = new GetUserQuery { Auth0Id = "" };

        // ACT / ASSERT
        await Should.ThrowAsync<Exception>(() => handler.Handle(query), $"Error getting user: {exceptionMessage}");
    }
}