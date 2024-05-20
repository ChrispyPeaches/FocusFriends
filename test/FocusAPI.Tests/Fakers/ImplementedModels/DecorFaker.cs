using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using FocusAPI.Models;

namespace FocusAPI.Tests.Fakers.ImplementedModels
{
    internal class DecorFaker : Faker<Decor>
    {
        public DecorFaker() 
        {
            RuleFor(decor => decor.Id, f => f.Random.Guid());
            RuleFor(decor => decor.Price, f => Math.Abs(f.Random.Int()));
            RuleFor(decor => decor.Name, f => f.Random.Word());
            RuleFor(decor => decor.HeightRequest, f => Math.Abs(f.Random.Int()));
            RuleFor(decor => decor.Image, f => [0x1]);
        }
    }
}
