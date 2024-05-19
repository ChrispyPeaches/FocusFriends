using Bogus;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers;
internal class BaseUserPetFaker : Faker<BaseUserPet>
{
    internal BaseUserPetFaker(Guid? userId = null)
    {
        RuleFor(userPet => userPet.UserId, f => userId ??= f.Random.Guid());
        RuleFor(userPet => userPet.PetId, f => f.Random.Guid());
    }
}