
namespace FocusCore.Models;
public class LeaderboardDto
{
    public string UserName { get; set; }
    public byte[] ProfilePicture { get; set; }
    public int CurrencyEarned { get; set; }
    public int Rank { get; set; }
}