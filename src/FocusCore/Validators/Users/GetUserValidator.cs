﻿using FluentValidation;
using FocusCore.Queries.User;

namespace FocusCore.Validators.Users;
public class GetUserValidator : AbstractValidator<GetUserQuery>
{
    public GetUserValidator()
    {
        RuleFor(user => user.Auth0Id)
            .NotNull()
            .NotEmpty();
    }
}