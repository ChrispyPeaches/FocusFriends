using Bogus;
using FocusAPI.Models;

namespace FocusAPI.Tests.Fakers;
internal class UserFaker : Faker<User>
{
    internal UserFaker()
    { 
    }
}