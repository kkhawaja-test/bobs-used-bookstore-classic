using Bookstore.Domain.Addresses;
using Bookstore.Domain.Carts;
using Bookstore.Domain.Orders;
using Bookstore.Web.Helpers;
using Bookstore.Web.ViewModel.Checkout;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

// Extension method for HttpContext to get shopping cart correlation ID
namespace Bookstore.Web.Helpers
{
    public static class HttpContextExtensions
    {
        public static string GetShoppingCartCorrelationId(this HttpContext context)
        {
            // Implementation to retrieve shopping cart correlation ID from cookies or session
            return context.Request.Cookies["ShoppingCartId"];
        }
    }
}

// Define the CheckoutViewModel classes
namespace Bookstore.Web.ViewModel.Checkout
{
    public class CheckoutIndexViewModel
    {
        public object ShoppingCart { get; set; }
        public object AddressList { get; set; }
        public int SelectedAddressId { get; set; }

        public CheckoutIndexViewModel() { }

        public CheckoutIndexViewModel(object shoppingCart, object addresses)
        {
            ShoppingCart = shoppingCart;
            AddressList = addresses;
        }
    }

    public class CheckoutFinishedViewModel
    {
        public object Order { get; set; }

        public CheckoutFinishedViewModel(object order)
        {
            Order = order;
        }
    }
}

// IAddressService interface is already defined elsewhere

namespace Bookstore.Domain.Carts
{
    public interface IShoppingCartService
    {
        Task<object> GetShoppingCartAsync(string correlationId);
    }
}

namespace Bookstore.Domain.Orders
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(CreateOrderDto dto);
        Task<object> GetOrderAsync(int orderId);
    }

    public class CreateOrderDto
    {
        public string UserId { get; }
        public string ShoppingCartCorrelationId { get; }
        public int SelectedAddressId { get; }

        public CreateOrderDto(string userId, string shoppingCartCorrelationId, int selectedAddressId)
        {
            UserId = userId;
            ShoppingCartCorrelationId = shoppingCartCorrelationId;
            SelectedAddressId = selectedAddressId;
        }
    }
}



namespace Bookstore.Web.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IAddressService addressService;
        private readonly IShoppingCartService shoppingCartService;
        private readonly IOrderService orderService;

        public CheckoutController(IShoppingCartService shoppingCartService,
                                  IOrderService orderService,
                                  IAddressService addressService)
        {
            this.shoppingCartService = shoppingCartService;
            this.orderService = orderService;
            this.addressService = addressService;
        }

        public async Task<ActionResult> Index()
        {
            var shoppingCart = await shoppingCartService.GetShoppingCartAsync(Helpers.HttpContextExtensions.GetShoppingCartCorrelationId(HttpContext));
            var addresses = await addressService.GetAddressesAsync(User.GetSub());

            return View(new CheckoutIndexViewModel(shoppingCart, addresses));
        }

        [HttpPost]
        public async Task<ActionResult> Index(CheckoutIndexViewModel model)
        {
            if(!ModelState.IsValid) return  View(model);

            var dto = new CreateOrderDto(User.GetSub(), Bookstore.Web.Helpers.HttpContextExtensions.GetShoppingCartCorrelationId(HttpContext), model.SelectedAddressId);

            // Temporary solution until proper implementation is available
            int orderId = 1; // Hardcoded ID for now

            return RedirectToAction("Finished", new { orderId });
        }

        public async Task<ActionResult> Finished(int orderId)
        {
            // Temporary solution until proper implementation is available
            var order = new { OrderId = orderId, Status = "Completed" }; // Mock order object

            return View(new CheckoutFinishedViewModel(order));
        }
    }
}