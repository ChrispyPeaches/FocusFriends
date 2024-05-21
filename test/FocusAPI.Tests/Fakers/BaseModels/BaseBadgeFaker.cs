using Bogus;
using FocusCore.Models;


namespace FocusAPI.Tests.Fakers.BaseModels
{
    internal class BaseBadgeFaker : Faker<BaseBadge>
    {
        public BaseBadgeFaker() 
        {
            RuleFor(badge => badge.Id, f => f.Random.Guid());
            RuleFor(badge => badge.Name, f => f.Random.Word());
            RuleFor(badge => badge.Description, f => f.Random.Words(4));
            RuleFor(badge => badge.Image, f => [0x1]);
        }
    }
}
