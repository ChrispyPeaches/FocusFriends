using Bogus;
using FocusAPI.Tests.Fakers.ImplementedModels;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers.BaseModels;
internal class BaseUserPetFaker : Faker<BaseUserPet>
{
    internal BaseUserPetFaker(Guid? userId = null)
    {
        BasePetFaker basePetFaker = new();
        BasePet pet = basePetFaker.Generate();
        RuleFor(userPet => userPet.UserId, f => userId ??= f.Random.Guid());
        RuleFor(userPet => userPet.PetId, f => pet.Id);
        RuleFor(userPet => userPet.Pet, f => pet);
    }
}