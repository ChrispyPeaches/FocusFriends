using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusCore.Commands.Social;
public class DeleteFriendCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid FriendId { get; set; }
}