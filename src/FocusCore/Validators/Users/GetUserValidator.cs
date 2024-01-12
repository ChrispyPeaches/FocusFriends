using FluentValidation;
using FocusCore.Commands.User;
using FocusCore.Queries.User;

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