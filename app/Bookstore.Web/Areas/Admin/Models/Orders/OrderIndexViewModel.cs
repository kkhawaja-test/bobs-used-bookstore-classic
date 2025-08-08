using Bookstore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.Areas.Admin.Models.Orders
{
    // Define necessary interfaces and classes for compilation
    public interface IPaginatedList<T> : IEnumerable<T>
    {
        int PageIndex { get; }
        int Count { get; }
        int TotalPages { get; }
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
        IEnumerable<int> GetPageList(int count);
    }

    // Minimal Order class definition for compilation
    public class Order
    {
        public int Id { get; set; }
        public Customer Customer { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime DeliveryDate { get; set; }
        public decimal Total { get; set; }
    }

    // Customer class needed for Order
    public class Customer
    {
        public string FullName { get; set; }
    }
    // Define the OrderStatus enum since it's not accessible
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public class OrderFilters
    {
        public string CustomerName { get; set; }
        public OrderStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SortBy { get; set; }
        public bool SortAscending { get; set; } = true;
    }

    public class OrderIndexViewModel : PaginatedViewModel
    {
        public List<OrderIndexListItemViewModel> Items { get; set; } = new List<OrderIndexListItemViewModel>();

        public OrderFilters Filters { get; set; }

        public OrderIndexViewModel(IPaginatedList<Order> orderDtos, OrderFilters filters)
        {
            foreach (var order in orderDtos)
            {
                Items.Add(new OrderIndexListItemViewModel
                {
                    Id = order.Id,
                    CustomerName = order.Customer.FullName,
                    OrderStatus = order.OrderStatus,
                    OrderDate = order.CreatedOn,
                    DeliveryDate = order.DeliveryDate,
                    Total = order.Total
                });
            }

            Filters = filters;

            PageIndex = orderDtos.PageIndex;
            PageSize = orderDtos.Count;
            PageCount = orderDtos.TotalPages;
            HasNextPage = orderDtos.HasNextPage;
            HasPreviousPage = orderDtos.HasPreviousPage;
            PaginationButtons = orderDtos.GetPageList(5).ToList();
        }
    }

    public class OrderIndexListItemViewModel
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public DateTime DeliveryDate { get; set; }

        public DateTime OrderDate { get; internal set; }

        public decimal Total { get; internal set; }
    }
}