using Bookstore.Domain;
using Bookstore.Domain.Orders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.ViewModel.Orders
{
    // Define the Order class if it doesn't exist in the referenced assemblies
    public class Order
    {
        public int Id { get; set; }
        public DateTime DeliveryDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal SubTotal { get; set; }
    }

    // Define OrderStatus enum if it doesn't exist
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    // Extension method for OrderStatus
    public static class OrderStatusExtensions
    {
        public static string GetDescription(this OrderStatus status)
        {
            return status.ToString();
        }
    }

    public class OrderIndexViewModel
    {
        public List<OrderIndexItemViewModel> OrderItems { get; set; } = new List<OrderIndexItemViewModel>();

        public OrderIndexViewModel(IEnumerable<Order> orders)
        {
            OrderItems = orders.Select(x => new OrderIndexItemViewModel
            {
                Id = x.Id,
                DeliveryDate = x.DeliveryDate,
                OrderStatus = x.OrderStatus.GetDescription(),
                SubTotal = x.SubTotal
            }).ToList();
        }
    }

    public class OrderIndexItemViewModel
    {
        public int Id { get; set; }

        public decimal SubTotal { get; set; }

        public DateTime DeliveryDate { get; set; }

        public string OrderStatus { get; set; }
    }
}