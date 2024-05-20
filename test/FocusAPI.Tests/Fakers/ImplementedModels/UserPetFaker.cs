using Bogus;
using FocusAPI.Models;

namespace FocusAPI.Tests.Fakers.ImplementedModels;
internal class UserPetFaker : Faker<UserPet>
{
    internal UserPetFaker(Guid? userId = null)
    {
        PetFaker petFaker = new();
        Pet pet = petFaker.Generate();
        RuleFor(userPet => userPet.UserId, f => userId ??= f.Random.Guid());
        RuleFor(userPet => userPet.PetId, f => pet.Id);
        RuleFor(userPet => userPet.Pet, f => pet);
    }
}