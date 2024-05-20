using Bogus;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers.BaseModels;
internal class BaseUserDecorFaker : Faker<BaseUserDecor>
{
    internal BaseUserDecorFaker(Guid? userId = null)
    {
        RuleFor(userDecor => userDecor.UserId, f => userId ??= f.Random.Guid());
        RuleFor(userDecor => userDecor.DecorId, f => f.Random.Guid());
    }
}