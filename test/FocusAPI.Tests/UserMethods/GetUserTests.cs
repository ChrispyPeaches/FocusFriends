using System.Net;
using FocusAPI.Data;
using FocusAPI.Methods;
using FocusAPI.Methods.User;
using FocusAPI.Models;
using FocusAPI.Tests.Fakers;
using FocusAPI.Tests.StaticHelpers;
using FocusCore.Queries.User;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;

namespace FocusAPI.Tests.UserMethods;
public class GetUserTests
{
    Mock<FocusAPIContext> _context;
    UserFaker _userFaker;
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
        _context = new Mock<FocusAPIContext>();
    }

    void SetupMocks(List<User> users)
    {
        // Set up mock db sets
        MockSetHelper.SetupEntities(users, _context, db => db.Users);
    }

    [Fact]
    public async Task GetUser_ReturnsNotFound_WhenUserIsNull()
    {
        // ARRANGE
        // Set up test data, test mocks, and system under test
        User user = _userFaker.Generate();
        SetupMocks([]);
        GetUser.Handler handler = new(_context.Object);

        // ACT
        var result = await handler.Handle(new GetUserQuery 
        {
            Auth0Id = user.Auth0Id
        });

        // ASSERT
        result.HttpStatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
