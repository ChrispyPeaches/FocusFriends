using Bogus;
using FocusAPI.Models;

namespace FocusAPI.Tests.Fakers.ImplementedModels
{
    internal class FriendshipFaker : Faker<Friendship>
    {
        public FriendshipFaker(User? user = null) 
        {
            RuleFor(friendship => friendship.UserId, f => user == null ? f.Random.Guid() : user.Id);
            RuleFor(friendship => friendship.FriendId, f => f.Random.Guid());
            RuleFor(friendship => friendship.Status, f => 1);
        }
    }
}
