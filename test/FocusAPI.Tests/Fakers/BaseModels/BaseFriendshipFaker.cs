using Bogus;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers.BaseModels
{
    internal class BaseFriendshipFaker : Faker<BaseFriendship>
    {
        public BaseFriendshipFaker(BaseUser? user = null) 
        {
            RuleFor(friendship => friendship.UserId, f => user == null ? f.Random.Guid() : user.Id);
            RuleFor(friendship => friendship.FriendId, f => f.Random.Guid());
            RuleFor(friendship => friendship.Status, f => 1);
        }
    }
}
