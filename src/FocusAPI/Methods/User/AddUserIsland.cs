using FocusCore.Commands.User;
using FocusAPI.Models;
using MediatR;
using FocusAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Methods.User;
public class AddUserIsland
{
    public class Handler : IRequestHandler<AddUserIslandCommand, Unit>
    {
        FocusAPIContext _apiContext;
        ILogger<Handler> _logger;
        public Handler(FocusAPIContext apiContext, ILogger<Handler> logger)
        {
            _apiContext = apiContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddUserIslandCommand command, CancellationToken cancellationToken)
        {
            try
            {
                Models.User user = await _apiContext.Users.FirstOrDefaultAsync(u => u.Id == command.UserId);
                Island island = await _apiContext.Islands.FirstOrDefaultAsync(s => s.Id == command.IslandId);

                user.Islands.Add(new UserIsland
                {
                    Island = island,
                    DateAcquired = DateTime.UtcNow
                });

                user.Balance = command.UpdatedBalance;

                await _apiContext.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error adding UserIsland to database.");
            }

            return Unit.Value;
        }
    }
}