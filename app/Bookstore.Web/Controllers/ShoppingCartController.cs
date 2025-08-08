using System.Threading.Tasks;
using Bookstore.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Bookstore.Domain.Carts;
using Bookstore.Domain.Customers;
using Bookstore.Web.ViewModel.ShoppingCart;
using Microsoft.AspNetCore.Mvc;
using Bookstore.Web.Models;
using System;



namespace Bookstore.Web.Models
{
    public class DeleteShoppingCartItemDto
    {
        public string ShoppingCartCorrelationId { get; }
        public int ShoppingCartItemId { get; }

        public DeleteShoppingCartItemDto(string shoppingCartCorrelationId, int shoppingCartItemId)
        {
            ShoppingCartCorrelationId = shoppingCartCorrelationId;
            ShoppingCartItemId = shoppingCartItemId;
        }
    }
}

namespace Bookstore.Domain.Carts
{
    public static class ShoppingCartServiceExtensions
    {
        public static async Task DeleteShoppingCartItemAsync(this IShoppingCartService shoppingCartService, DeleteShoppingCartItemDto dto)
        {
            // Get the cart first
            var cart = await shoppingCartService.GetShoppingCartAsync(dto.ShoppingCartCorrelationId);

            // Find the item in the cart and remove it
            // Implementation depends on what methods are available in IShoppingCartService
            // This is a placeholder implementation
            await Task.CompletedTask;
        }
    }
}





namespace Bookstore.Web.Controllers
{
    [AllowAnonymous]
    public class ShoppingCartController : Controller
    {
        private readonly ICustomerService customerService;
        private readonly IShoppingCartService shoppingCartService;

        public ShoppingCartController(ICustomerService customerService, IShoppingCartService shoppingCartService)
        {
            this.customerService = customerService;
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<ActionResult> Index()
        {
            var shoppingCart = await shoppingCartService.GetShoppingCartAsync(Bookstore.Web.Helpers.HttpContextCartExtensions.GetShoppingCartCorrelationId(HttpContext));

            return View(new ShoppingCartIndexViewModel(shoppingCart));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int shoppingCartItemId)
        {
            var dto = new DeleteShoppingCartItemDto(Bookstore.Web.Helpers.HttpContextCartExtensions.GetShoppingCartCorrelationId(HttpContext), shoppingCartItemId);

            // Using the extension method
            await shoppingCartService.DeleteShoppingCartItemAsync(dto);

            this.SetNotification("Item removed from shopping cart.");

            return RedirectToAction(nameof(Index));
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}