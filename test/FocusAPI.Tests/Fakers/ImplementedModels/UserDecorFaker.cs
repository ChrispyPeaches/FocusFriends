using Bogus;
using FocusAPI.Models;

namespace FocusAPI.Tests.Fakers.ImplementedModels;
internal class UserDecorFaker : Faker<UserDecor>
{
    internal UserDecorFaker(Guid? userId = null)
    {
        DecorFaker decorFaker = new();
        Decor decor = decorFaker.Generate();
        RuleFor(userDecor => userDecor.UserId, f => userId ??= f.Random.Guid());
        RuleFor(userDecor => userDecor.DecorId, f => decor.Id);
        RuleFor(userDecor => userDecor.Decor, f => decor);
    }
}