using Microsoft.AspNetCore.Mvc;
using MediatR;
using FocusAPI.Models;
using FocusCore.Queries.Shop;
using FocusCore.Models;

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
        public async Task<List<ShopItem>> GetAllShopItems()
        {
            return await _mediator.Send(new GetAllShopItemsQuery());
        }
    }
}