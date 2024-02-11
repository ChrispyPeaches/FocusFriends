using Microsoft.AspNetCore.Mvc;
using MediatR;
using FocusAPI.Models;
using FocusAPI.Queries.Shop;

namespace FocusAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShopController : ControllerBase
    {
        private readonly ILogger<ShopController> _logger;
        private IMediator _mediator;

        public ShopController(ILogger<ShopController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<List<Pet>> GetAllShopItems()
        {
            return await _mediator.Send(new GetAllShopItemsQuery());
        }
    }
}