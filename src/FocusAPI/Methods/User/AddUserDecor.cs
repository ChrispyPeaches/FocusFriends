using FocusCore.Commands.User;
using FocusAPI.Models;
using MediatR;
using FocusAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FocusAPI.Methods.User;
public class AddUserDecor
{
    public class Handler : IRequestHandler<AddUserDecorCommand, Unit>
    {
        FocusAPIContext _apiContext;
        ILogger<Handler> _logger;
        public Handler(FocusAPIContext apiContext, ILogger<Handler> logger)
        {
            _apiContext = apiContext;
            _logger = logger;
        }

        public async Task<Unit> Handle(AddUserDecorCommand command, CancellationToken cancellationToken)
        {
            try
            {
                FocusAPI.Models.User user = await _apiContext.Users.FirstOrDefaultAsync(u => u.Id == command.UserId);
                FocusAPI.Models.Decor decor = await _apiContext.Decor.FirstOrDefaultAsync(f => f.Id == command.DecorId);

                user.Decor.Add(new UserDecor
                {
                    Decor = decor,
                    DateAcquired = DateTime.UtcNow
                });

                user.Balance = command.UpdatedBalance;

                await _apiContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "Error adding UserDecor to database. Exception: " + ex.Message);
            }

            return Unit.Value;
        }
    }
}