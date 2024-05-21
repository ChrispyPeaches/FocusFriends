using Bogus;
using FocusAPI.Tests.Fakers.ImplementedModels;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers.BaseModels;
internal class BaseUserBadgeFaker : Faker<BaseUserBadge>
{
    internal BaseUserBadgeFaker(Guid? userId = null)
    {
        BaseBadgeFaker baseBadgeFaker = new();
        BaseBadge badge = baseBadgeFaker.Generate();
        RuleFor(userBadge => userBadge.UserId, f => userId ??= f.Random.Guid());
        RuleFor(userBadge => userBadge.BadgeId, f => badge.Id);
        RuleFor(userBadge => userBadge.Badge, f => badge);
    }
}