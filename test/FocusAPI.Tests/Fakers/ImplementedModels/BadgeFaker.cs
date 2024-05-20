using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using FocusAPI.Models;


namespace FocusAPI.Tests.Fakers.ImplementedModels
{
    internal class BadgeFaker : Faker<Badge>
    {
        public BadgeFaker() 
        {
            RuleFor(badge => badge.Id, f => f.Random.Guid());
            RuleFor(badge => badge.Name, f => f.Random.Word());
            RuleFor(badge => badge.Description, f => f.Random.Words(4));
            RuleFor(badge => badge.Image, f => [0x1]);
        }
    }
}
