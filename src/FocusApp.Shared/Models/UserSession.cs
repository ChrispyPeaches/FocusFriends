namespace FocusApp.Shared.Models;

public class UserSession : FocusCore.Models.BaseUserSession
{
    public new User? User { get; set; }

    public new DateTime SessionStartTime { get; set; }

    public new DateTime SessionEndTime { get; set; }
}
