using Bogus;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers.ImplementedModels
{
    internal class BasePetFaker : Faker<BasePet>
    {
        public BasePetFaker()
        {
            RuleFor(pet => pet.Id, f => f.Random.Guid());
            RuleFor(pet => pet.Price, f => Math.Abs(f.Random.Int()));
            RuleFor(pet => pet.Name, f => f.Random.Word());
            RuleFor(pet => pet.Image, f => [0x1]);
            RuleFor(pet => pet.HeightRequest, f => Math.Abs(f.Random.Int()));
        }
    }
}
