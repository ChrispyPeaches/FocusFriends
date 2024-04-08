using FocusCore.Models;

namespace FocusCore.Extensions;

public static class UserExtensions
{
    public static string FullName(this BaseUser user)
    {
        return user.FirstName +
               (
                   !string.IsNullOrEmpty(user.LastName)
                       ? $" {user.LastName}"
                       : ""
               );
    }
}