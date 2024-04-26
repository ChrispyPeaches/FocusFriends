using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FocusAPI.Data;
using FocusAPI.Models;
using FocusCore.Commands.User;
using FocusCore.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FocusAPI.Methods.User;
public class EditUserSelectedIsland
{
    public class Handler : IRequestHandler<EditUserSelectedIslandCommand, MediatrResult>
    {
        FocusAPIContext _context;
        ILogger<Handler> _logger;
        public Handler(FocusAPIContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MediatrResult> Handle(EditUserSelectedIslandCommand command, CancellationToken cancellationToken) 
        {
            Models.User? user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == command.UserId);

            if (user == null)
                return new MediatrResult
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    Message = "User not found"
                };

            try
            {
                Island island = await _context.Islands.Where(d => command.IslandId == d.Id).FirstOrDefaultAsync(cancellationToken);

                // Update the user's selected island
                user.SelectedIsland = island;
                user.SelectedIslandId = island.Id;

                await _context.SaveChangesAsync();

                return new MediatrResult { HttpStatusCode = HttpStatusCode.OK };
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error saving changes to user selected island in database.");
                
                return new MediatrResult
                {
                    HttpStatusCode = HttpStatusCode.InternalServerError,
                    Message = "Error saving changes to user selected island in database."
                };
            }
        }
    }
}

