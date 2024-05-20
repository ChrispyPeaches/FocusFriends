using Bogus;
using FocusAPI.Models;

namespace FocusAPI.Tests.Fakers.ImplementedModels;
internal class UserBadgeFaker : Faker<UserBadge>
{
    internal UserBadgeFaker(Guid? userId = null)
    {
        BadgeFaker badgeFaker = new();
        Badge badge = badgeFaker.Generate();
        RuleFor(userBadge => userBadge.UserId, f => userId ??= f.Random.Guid());
        RuleFor(userBadge => userBadge.BadgeId, f => badge.Id);
        RuleFor(userBadge => userBadge.DateAcquired, f => f.Date.Recent());
        RuleFor(userBadge => userBadge.Badge, f => badge);
    }
}