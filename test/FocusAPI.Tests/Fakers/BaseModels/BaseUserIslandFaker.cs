using Bogus;
using FocusAPI.Models;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers.BaseModels;
internal class BaseUserIslandFaker : Faker<BaseUserIsland>
{
    internal BaseUserIslandFaker(Guid? userId = null)
    {
        RuleFor(userIsland => userIsland.UserId, f => userId ??= f.Random.Guid());
        RuleFor(userIsland => userIsland.IslandId, f => f.Random.Guid());
    }
}