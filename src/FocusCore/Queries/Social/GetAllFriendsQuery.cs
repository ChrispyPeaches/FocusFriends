using FocusCore.Models;
using FocusApp.Shared.Models;
using MediatR;

namespace FocusCore.Queries.Social;
public class GetAllFriendsQuery : IRequest<List<FriendShip>>
{ 
    public Guid UserId { get; set; }
}

