﻿using System.Net;
using FocusCore.Commands.User;
using MediatR;
using FocusAPI.Data;
using FocusAPI.Helpers;
using FocusAPI.Models;
using FocusCore.Models;
using FocusCore.Responses.User;
using Microsoft.EntityFrameworkCore;
using FocusCore.Responses;

namespace FocusApi.Methods.User;
public class CreateUser
{
    public class Handler : IRequestHandler<CreateUserCommand, MediatrResultWrapper<CreateUserResponse>>
    {
        private readonly FocusContext _context;
        private readonly ISyncService _syncService;

        public Handler(
            FocusContext context,
            ISyncService syncService)
        {
            _context = context;
            _syncService = syncService;
        }

        public async Task<MediatrResultWrapper<CreateUserResponse>> Handle(
            CreateUserCommand command,
            CancellationToken cancellationToken = default)
        {
            FocusAPI.Models.User? existingUser = await GetUser(command, cancellationToken);
               
            // If the user does not yet exist in the database, create the user
            if (existingUser == null)
            {
                Island? initialIsland = await GetInitialIsland(cancellationToken);
                Pet? initialPet = await GetInitialPet(cancellationToken);

                BaseUser? newUser = await CreateUser(command, initialPet, initialIsland, cancellationToken);

                return new MediatrResultWrapper<CreateUserResponse>()
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Data = new CreateUserResponse { User = newUser }
                };
            }
            else
            {
                return new MediatrResultWrapper<CreateUserResponse>
                { 
                    HttpStatusCode = HttpStatusCode.Conflict, 
                    Message = $"User already exists with Auth0Id: {command.Auth0Id}",
                };
            }

            
        }

        private async Task<FocusAPI.Models.User?> GetUser(
            CreateUserCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.Users
                    .Where(u => u.Auth0Id == command.Auth0Id)
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting user");
            }
        }

        private async Task<Island?> GetInitialIsland(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _syncService
                    .GetInitialIslandQuery()
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting initial island");
            }
        }

        private async Task<Pet?> GetInitialPet(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _syncService
                    .GetInitialPetQuery()
                    .FirstOrDefaultAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting initial pet");
            }
        }

        private async Task<BaseUser> CreateUser(
            CreateUserCommand command,
            Pet? initialPet,
            Island? initialIsland,
            CancellationToken cancellationToken)
        {
            FocusAPI.Models.User user = new FocusAPI.Models.User
            {
                Auth0Id = command.Auth0Id,
                Id = Guid.NewGuid(),
                UserName = command.UserName,
                Email = command.Email,
                Balance = 0,
                DateCreated = DateTime.UtcNow
            };

            if (initialPet != null)
            {
                user.Pets?.Add(new UserPet() { Pet = initialPet });
            }

            if (initialIsland != null)
            {
                user.Islands?.Add(new UserIsland() { Island = initialIsland });
            }

            _context.Users.Add(user);

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating user");
            }

            return user;
        }
    }
}