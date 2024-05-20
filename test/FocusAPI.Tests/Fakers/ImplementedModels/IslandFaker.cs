using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using FocusAPI.Models;

namespace FocusAPI.Tests.Fakers.ImplementedModels
{
    internal class IslandFaker : Faker<Island>
    {
        public IslandFaker() 
        {
            RuleFor(island => island.Id, f => f.Random.Guid());
            RuleFor(island => island.Price, f => Math.Abs(f.Random.Int()));
            RuleFor(island => island.Name, f => f.Random.Word());
            RuleFor(island => island.Image, f => [0x1]);
        }
    }
}
