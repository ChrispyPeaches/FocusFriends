using Bogus;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers.BaseModels;
internal class BaseUserBadgeFaker : Faker<BaseUserBadge>
{
    internal BaseUserBadgeFaker(Guid? userId = null)
    {
        RuleFor(userBadge => userBadge.UserId, f => userId ??= f.Random.Guid());
        RuleFor(userBadge => userBadge.BadgeId, f => f.Random.Guid());
    }
}