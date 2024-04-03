using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusCore.Commands.Social;
public class CreateFriendRequestCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid FriendId { get; set; }
}