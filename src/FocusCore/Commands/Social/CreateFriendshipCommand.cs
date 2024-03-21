using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusCore.Commands.Social;
public class CreateFriendshipCommand : IRequest<Unit>
{
    public string Name { get; set; }
}