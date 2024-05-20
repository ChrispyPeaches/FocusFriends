using Bogus;
using FocusAPI.Models;

namespace FocusAPI.Tests.Fakers.ImplementedModels;
internal class UserIslandFaker : Faker<UserIsland>
{
    internal UserIslandFaker(Guid? userId = null)
    {
        IslandFaker islandFaker = new();
        Island island = islandFaker.Generate();
        RuleFor(userIsland => userIsland.UserId, f => userId ??= f.Random.Guid());
        RuleFor(userIsland => userIsland.IslandId, f => island.Id);
        RuleFor(userIsland => userIsland.Island, f => island);
    }
}