using Bogus;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers.BaseModels;
internal class BaseUserDecorFaker : Faker<BaseUserDecor>
{
    internal BaseUserDecorFaker(Guid? userId = null)
    {
        BaseDecorFaker baseDecorFaker = new();
        BaseDecor decor = baseDecorFaker.Generate();
        RuleFor(userDecor => userDecor.UserId, f => userId ??= f.Random.Guid());
        RuleFor(userDecor => userDecor.DecorId, f => decor.Id);
        RuleFor(userDecor => userDecor.Decor, f => decor);
    }
}