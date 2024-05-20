using Bogus;
using FocusAPI.Models;

namespace FocusAPI.Tests.Fakers.ImplementedModels
{
    internal class PetFaker : Faker<Pet>
    {
        public PetFaker()
        {
            RuleFor(pet => pet.Id, f => f.Random.Guid());
            RuleFor(pet => pet.Price, f => Math.Abs(f.Random.Int()));
            RuleFor(pet => pet.Name, f => f.Random.Word());
            RuleFor(pet => pet.Image, f => [0x1]);
            RuleFor(pet => pet.HeightRequest, f => Math.Abs(f.Random.Int()));
        }
    }
}
