using FocusCore.Models;
using MediatR;

namespace FocusCore.Queries.Social;
public class GetAllFriendsQuery : IRequest<List<FriendListModel>>
{ 
    public Guid UserId { get; set; }
}

