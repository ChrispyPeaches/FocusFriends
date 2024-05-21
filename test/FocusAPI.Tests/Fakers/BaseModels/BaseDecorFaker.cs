using Bogus;
using FocusCore.Models;

namespace FocusAPI.Tests.Fakers.BaseModels
{
    internal class BaseDecorFaker : Faker<BaseDecor>
    {
        public BaseDecorFaker() 
        {
            RuleFor(decor => decor.Id, f => f.Random.Guid());
            RuleFor(decor => decor.Price, f => Math.Abs(f.Random.Int()));
            RuleFor(decor => decor.Name, f => f.Random.Word());
            RuleFor(decor => decor.HeightRequest, f => Math.Abs(f.Random.Int()));
            RuleFor(decor => decor.Image, f => [0x1]);
        }
    }
}
