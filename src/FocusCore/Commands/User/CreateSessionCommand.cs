using FocusCore.Responses;
using MediatR;

namespace FocusCore.Commands.User;

public class CreateSessionCommand : IRequest<MediatrResult>
{
    public string? Auth0Id { get; set; }
    public Guid SessionId { get; set; }
    public DateTimeOffset SessionStartTime { get; set; }
    public DateTimeOffset SessionEndTime { get; set; }
    public int CurrencyEarned { get; set; }
}