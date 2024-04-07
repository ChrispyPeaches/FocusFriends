using FocusCore.Models;
using MediatR;

namespace FocusCore.Queries.Social;
public class GetAllFriendRequestsQuery : IRequest<List<FriendRequest>>
{ 
    public Guid UserId { get; set; }
}

