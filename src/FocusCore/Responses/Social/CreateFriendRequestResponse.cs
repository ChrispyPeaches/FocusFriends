using FocusCore.Models;

namespace FocusCore.Responses.Social;

public class CreateFriendRequestResponse
{
    public BaseFriendship? PendingFriendRequest { get; set; }
}