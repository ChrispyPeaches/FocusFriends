using FocusApp.Client.Clients;
using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Commands.User;
using FocusCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FocusApp.Client.Methods.Shop
{
    internal class PurchaseItem
    {
        public class Command : IRequest<Unit>
        { 
            public ShopItem Item { get; set; }
        }

        public class Handler : IRequestHandler<Command, Unit>
        {
            FocusAppContext _localContext;
            IAPIClient _client;
            IAuthenticationService _authenticationService;
            ILogger<Handler> _logger;
            public Handler(FocusAppContext localContext, IAPIClient client, IAuthenticationService authenticationService, ILogger<Handler> logger)
            {
                _localContext = localContext;
                _client = client;
                _authenticationService = authenticationService;
                _logger = logger;
            }

            public async Task<Unit> Handle(Command command, CancellationToken cancellationToken)
            {
                switch (command.Item.Type)
                {
                    case ShopItemType.Pets:
                        try
                        {
                            // Add the user's new pet to the local database
                            Shared.Models.User user = await _localContext.Users.FirstAsync(u => u.Id == _authenticationService.CurrentUser.Id, cancellationToken);
                            user.Pets?.Add(new UserPet
                            {
                                Pet = await _localContext.Pets.FirstAsync(p => p.Id == command.Item.Id, cancellationToken)
                            });

                            // Update the user's balance on the local database
                            user.Balance = _authenticationService.Balance;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error adding UserPet to local database.");
                        }

                        // Add the user's pet to the server database
                        // Note: This endpoint additionally updates the user's balance on the server
                        try
                        {
                            await _client.AddUserPet(new AddUserPetCommand
                            {
                                UserId = _authenticationService.CurrentUser.Id,
                                PetId = command.Item.Id,
                                UpdatedBalance = _authenticationService.Balance,
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error adding UserPet to server database.");
                        }

                        break;

                    case ShopItemType.Decor:
                        // Add the user's new decor to the local database
                        try
                        {
                            Shared.Models.User user = await _localContext.Users.FirstAsync(u => u.Id == _authenticationService.CurrentUser.Id, cancellationToken);
                            user.Decor?.Add(new UserDecor
                            {
                                Decor = await _localContext.Decor.FirstAsync(d => d.Id == command.Item.Id, cancellationToken)
                            });

                            // Update the user's balance on the local database
                            user.Balance = _authenticationService.Balance;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error adding UserDecor to local database.");
                        }

                        // Add the user's decor to the server database
                        // Note: This endpoint additionally updates the user's balance on the server
                        try
                        {
                            await _client.AddUserDecor(new AddUserDecorCommand
                            {
                                UserId = _authenticationService.CurrentUser.Id,
                                DecorId = command.Item.Id,
                                UpdatedBalance = _authenticationService.Balance,
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error adding UserDecor to server database.");
                        }

                        break;
                    case ShopItemType.Islands:
                        try
                        {
                            // Add the user's new island to the local database
                            Shared.Models.User user = await _localContext.Users.FirstAsync(u => u.Id == _authenticationService.CurrentUser.Id, cancellationToken);
                            user.Islands?.Add(new UserIsland
                            {
                                Island = await _localContext.Islands.FirstAsync(i => i.Id == command.Item.Id, cancellationToken)
                            });

                            // Update the user's balance on the local database
                            user.Balance = _authenticationService.Balance;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error adding UserIsland to local database.");
                        }

                        // Add the user's island to the server database
                        try
                        {
                            await _client.AddUserIsland(new AddUserIslandCommand
                            {
                                UserId = _authenticationService.CurrentUser.Id,
                                IslandId = command.Item.Id,
                                UpdatedBalance = _authenticationService.Balance,
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error adding UserIsland to server database.");
                        }

                        break;

                    default:
                        break;
                }

                try
                {
                    await _localContext.SaveChangesAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving changes to local database.");
                }

                return Unit.Value;
            }
        }
    }
}
