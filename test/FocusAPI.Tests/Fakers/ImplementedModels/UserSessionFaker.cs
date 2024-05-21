using Bogus;
using FocusAPI.Models;

namespace FocusAPI.Tests.Fakers.ImplementedModels
{
    internal class UserSessionFaker : Faker<UserSession>
    {
        internal UserSessionFaker(Guid? userId = null)
        {
            RuleFor(session => session.Id, f => f.Random.Guid());
            RuleFor(session => session.UserId, f => userId == null ? f.Random.Guid() : userId);
            RuleFor(session => session.SessionStartTime, f => f.Date.RecentOffset(1));
            RuleFor(session => session.SessionEndTime, f => DateTimeOffset.UtcNow);
            RuleFor(session => session.CurrencyEarned, f => Math.Abs(f.Random.Int()));
        }
    }
}
