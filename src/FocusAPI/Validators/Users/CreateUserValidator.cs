using FluentValidation;
using FocusAPI.Commands.User;

namespace FocusCore.Validators.Users;
public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(user => user.Name)
            .NotNull()
            .NotEmpty()
            .Length(1, 50);
    }
}