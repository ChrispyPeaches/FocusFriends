using FluentValidation;
using FocusCore.Commands.User;

namespace FocusCore.Validators.Users;
public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(user => user.Auth0Id)
            .NotNull()
            .NotEmpty()
            .Length(1, 50);
        RuleFor(user => user.UserName)
            .NotNull()
            .NotEmpty()
            .Length(1, 50);
        RuleFor(user => user.Email)
            .NotNull()
            .NotEmpty()
            .Length(1, 320)
            .EmailAddress();
    }
}