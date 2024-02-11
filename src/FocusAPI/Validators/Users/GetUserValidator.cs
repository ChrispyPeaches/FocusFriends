using FluentValidation;
using FocusAPI.Queries.User;

namespace FocusCore.Validators.Users;
public class GetUserValidator : AbstractValidator<GetUserQuery>
{
    public GetUserValidator()
    {
        RuleFor(user => user.Id)
            .NotNull()
            .NotEmpty();
    }
}