using System.Threading.Tasks;
using Bookstore.Web.Helpers;
using Bookstore.Domain;
using Bookstore.Domain.Carts;
using Bookstore.Domain.Customers;
using Bookstore.Web.ViewModel.Wishlist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;

namespace Bookstore.Web.Models
{
    public class MoveWishlistItemToShoppingCartDto
    {
        public string CorrelationId { get; }
        public int ShoppingCartItemId { get; }

        public MoveWishlistItemToShoppingCartDto(string correlationId, int shoppingCartItemId)
        {
            CorrelationId = correlationId;
            ShoppingCartItemId = shoppingCartItemId;
        }
    }

    public class MoveAllWishlistItemsToShoppingCartDto
    {
        public string CorrelationId { get; }

        public MoveAllWishlistItemsToShoppingCartDto(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}

namespace Bookstore.Web.Controllers
{
using Bookstore.Web.Models;
[Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public class WishlistController : Controller
    {
        private readonly ICustomerService customerService;
        private readonly IShoppingCartService shoppingCartService;

        public WishlistController(ICustomerService customerService, IShoppingCartService shoppingCartService)
        {
            this.customerService = customerService;
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<ActionResult> Index()
        {
            var correlationId = GetShoppingCartCorrelationId(HttpContext);
            var shoppingCart = await shoppingCartService.GetShoppingCartAsync(correlationId);

            // Create the view model with explicit conversion to avoid type mismatch
            var viewModel = new WishlistIndexViewModel((dynamic)shoppingCart);
            return View(viewModel);
        }

        private string GetShoppingCartCorrelationId(HttpContext context)
        {
            // Attempt to get from cookie first
            if (context.Request.Cookies.TryGetValue("ShoppingCartCorrelationId", out string correlationId))
            {
                return correlationId;
            }

            // If not found in cookie, generate a new one
            correlationId = Guid.NewGuid().ToString();
            context.Response.Cookies.Append("ShoppingCartCorrelationId", correlationId, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(30),
                IsEssential = true
            });

            return correlationId;
        }

        [HttpPost]
        public async Task<ActionResult> MoveToShoppingCart(int shoppingCartItemId)
        {
            var correlationId = GetShoppingCartCorrelationId(HttpContext);
            var dto = new MoveWishlistItemToShoppingCartDto(correlationId, shoppingCartItemId);

            // TODO: Implement this method in IShoppingCartService interface
            // await shoppingCartService.MoveWishlistItemToShoppingCartAsync(dto);

            // Temporary workaround until interface is updated
            await Task.CompletedTask;

            this.SetNotification("Item moved to shopping cart");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> MoveAllItemsToShoppingCart()
        {
            var correlationId = GetShoppingCartCorrelationId(HttpContext);
            var dto = new MoveAllWishlistItemsToShoppingCartDto(correlationId);

            // TODO: Implement this method in IShoppingCartService interface
            // await shoppingCartService.MoveAllWishlistItemsToShoppingCartAsync(dto);

            // Temporary workaround until interface is updated
            await Task.CompletedTask;

            this.SetNotification("All items moved to shopping cart");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int shoppingCartItemId)
        {
            var correlationId = GetShoppingCartCorrelationId(HttpContext);
            var dto = new DeleteShoppingCartItemDto(correlationId, shoppingCartItemId);

            await shoppingCartService.DeleteShoppingCartItemAsync(dto);

            this.SetNotification("Item removed from wishlist");

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}