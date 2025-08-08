using System;
using System.Threading.Tasks;
using Bookstore.Web.Areas.Admin.Models.Orders;
using Microsoft.AspNetCore.Mvc;


namespace Bookstore.Web.Areas.Admin.Controllers
{

    // Define the interfaces needed from Bookstore.Domain.Orders
    public interface IOrderService
    {
        Task<Models.Orders.IPaginatedList<Models.Orders.Order>> GetOrdersAsync(Models.Orders.OrderFilters filters, int pageIndex, int pageSize);
        Task<object> GetOrderAsync(int id);
        Task UpdateOrderStatusAsync(UpdateOrderStatusDto dto);
    }

    public class UpdateOrderStatusDto
    {
        public int OrderId { get; }
        public Models.Orders.OrderStatus SelectedOrderStatus { get; }

        public UpdateOrderStatusDto(int orderId, Models.Orders.OrderStatus selectedOrderStatus)
        {
            OrderId = orderId;
            SelectedOrderStatus = selectedOrderStatus;
        }
    }

    public class OrdersController : AdminAreaControllerBase
    {
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public async Task<ActionResult> Index(Models.Orders.OrderFilters filters, int pageIndex = 1, int pageSize = 10)
        {
            var orders = await orderService.GetOrdersAsync(filters, pageIndex, pageSize);

            return View(new OrderIndexViewModel(orders, filters));
        }

        public async Task<ActionResult> Details(int id)
        {
            var order = await orderService.GetOrderAsync(id);

            return View(new OrderDetailsViewModel((Models.Orders.Order)order));
        }

        [HttpPost]
        public async Task<ActionResult> Details(OrderDetailsViewModel model)
        {
            var dto = new UpdateOrderStatusDto(model.OrderId, model.SelectedOrderStatus);

            await orderService.UpdateOrderStatusAsync(dto);

            TempData["Message"] = "Order status has been updated";

            return RedirectToAction("Details", new { model.OrderId });
        }
    }
}