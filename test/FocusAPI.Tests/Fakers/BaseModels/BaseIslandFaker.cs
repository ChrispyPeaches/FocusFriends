using Bogus;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers.BaseModels
{
    internal class BaseIslandFaker : Faker<BaseIsland>
    {
        public BaseIslandFaker() 
        {
            RuleFor(island => island.Id, f => f.Random.Guid());
            RuleFor(island => island.Price, f => Math.Abs(f.Random.Int()));
            RuleFor(island => island.Name, f => f.Random.Word());
            RuleFor(island => island.Image, f => [0x1]);
        }
    }
}
