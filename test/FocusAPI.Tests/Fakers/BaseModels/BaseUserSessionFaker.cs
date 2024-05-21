using Bogus;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers.BaseModels
{
    internal class BaseUserSessionFaker : Faker<BaseUserSession>
    {
        internal BaseUserSessionFaker(Guid? userId = null)
        {
            RuleFor(session => session.Id, f => f.Random.Guid());
            RuleFor(session => session.UserId, f => userId == null ? f.Random.Guid() : userId);
            RuleFor(session => session.SessionStartTime, f => f.Date.RecentOffset(1));
            RuleFor(session => session.SessionEndTime, f => DateTimeOffset.UtcNow);
            RuleFor(session => session.CurrencyEarned, f => Math.Abs(f.Random.Int()));
        }
    }
}
