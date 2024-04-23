using FocusApp.Client.Helpers;
using FocusApp.Shared.Data;
using FocusApp.Shared.Models;
using FocusCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FocusApp.Client.Methods.User
{
    internal class GetUserPets
    {
        internal class Query : IRequest<Result> 
        {
            public Guid UserId { get; set; }
            public Guid selectedPetId { get; set; }
        }

        internal class Result
        {
            public List<PetItem> Pets { get; set; }
        }

        internal class Handler : IRequestHandler<Query, Result>
        {
            FocusAppContext _context;

            public Handler(FocusAppContext context)
            {
                _context = context;
            }

            public async Task<Result> Handle(
                Query query,
                CancellationToken cancellationToken = default)
            {
                List<PetItem> userPets = new List<PetItem>();

                Guid userSelectedPetId = query.selectedPetId;

                userPets = await _context.UserPets
                    .Include(p => p.Pet)
                    .Where(p => p.UserId == query.UserId)
                    .Select(p =>
                    new PetItem
                    {
                        PetId = p.PetId,
                        PetName = p.Pet.Name,
                        PetsProfilePicture = p.Pet.Image,

                        // Determine if pet is currently selected
                        isSelected = userSelectedPetId == p.PetId
                    })
                    .ToListAsync();

                return new Result
                {
                    Pets = userPets
                };
            }
        }
    }
}
