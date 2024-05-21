using Bogus;
using FocusAPI.Models;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers.BaseModels;
internal class BaseUserIslandFaker : Faker<BaseUserIsland>
{
    internal BaseUserIslandFaker(Guid? userId = null)
    {
        BaseIslandFaker baseIslandFaker = new();
        BaseIsland island = baseIslandFaker.Generate();
        RuleFor(userIsland => userIsland.UserId, f => userId ??= f.Random.Guid());
        RuleFor(userIsland => userIsland.IslandId, f => island.Id);
        RuleFor(userIsland => userIsland.Island, f => island);
    }
}