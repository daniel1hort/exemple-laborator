using L01.Domain;
using L01.Models;
using L01.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using static L01.Domain.CartState;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly PSSCContext context;
        private CartWrapper cart;

        public ShopController(PSSCContext context, CartWrapper cart)
        {
            this.context = context;
            this.cart = cart;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
            => Ok(context.Products.AsEnumerable().Select(a => new ProductViewModel(a)));

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
            => Ok(cart.Cart.Items.Select(a => new { Product = new ProductViewModel(a.Product), Quantity = a.Quantity }));

        [HttpPost("{id}/{quantity}")]
        public async Task<IActionResult> AddItem(int id, int quantity)
        {
            var result = L01.Domain.AppDomain.AddItemToCart(cart.Cart, CheckProduct(id, quantity));
            IActionResult response;
            (cart.Cart, response) = result switch
            {
                ValidCart a => (a.Cart, Ok()),
                InvalidCart a => (a.Cart, (IActionResult)BadRequest(a.Message))
            };
            return response;
        }

        [HttpPost("{address}")]
        public async Task<IActionResult> Pay(string address)
        {
            if (!cart.Cart.Items.Any())
                return BadRequest("Please add items to cart first");
            L01.Domain.AppDomain.PayCart(cart.Cart, address, context);
            cart.Cart = cart.Cart with { Items = Enumerable.Empty<(Product, int)>() };
            await context.SaveChangesAsync();
            return Ok();
        }

        private TryAsync<(Product, int)> CheckProduct(int id, int quantity) => async () =>
        {
            var product = context.Products.FirstOrDefault(a => a.Id == id);
            if(product is null)
                throw new Exception("The product doesn't exist");
            var oldQuantity = cart.Cart.Items.FirstOrDefault(a => a.Product.Id == id).Quantity;
            if (quantity <= 0)
                throw new Exception("Quantity must be bigger than 0");
            if (oldQuantity + quantity > product.Stoc)
                throw new Exception("Not enough items in stock");
            return (product, quantity);
        };
    }
}
