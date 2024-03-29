﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusCore.Commands.User;
public class CreateUserCommand : IRequest<Unit>
{
    public string Name { get; set; }
}