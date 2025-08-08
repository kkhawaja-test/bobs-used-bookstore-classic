using System;
using System.Collections.Generic;

// Import the actual Bookstore.Domain.Orders namespace
using Bookstore.Domain.Orders;
using System.Reflection;
using Bookstore.Web.Areas.Admin.Models.Orders.Models;

// Define local model classes to avoid conflicts with domain classes
namespace Bookstore.Web.Areas.Admin.Models.Orders.Models
{
    public class Address
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }

    public class Customer
    {
        public string FullName { get; set; }
    }

    public class OrderItem
    {
        public Book Book { get; set; }
    }

    public class Book
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public BookType BookType { get; set; }
        public Condition Condition { get; set; }
        public Genre Genre { get; set; }
        public decimal Price { get; set; }
        public Publisher Publisher { get; set; }
    }

    public class BookType
    {
        public string Text { get; set; }
    }

    public class Condition
    {
        public string Text { get; set; }
    }

    public class Genre
    {
        public string Text { get; set; }
    }

    public class Publisher
    {
        public string Text { get; set; }
    }
}

namespace Bookstore.Web.Areas.Admin.Models.Orders
{
    public class OrderDetailsViewModel
    {
        public int OrderId { get; set; }

        public OrderStatus SelectedOrderStatus { get; set; }

        public DateTime OrderDate { get; set; }

        public DateTime DeliveryDate { get; set; }

        public string CustomerName { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string Country { get; set; }

        public decimal Subtotal { get; set; }

        public decimal Tax { get; set; }

        public decimal Total => Subtotal + Tax;

        public List<OrderDetailsItemViewModel> Items { get; set; } = new List<OrderDetailsItemViewModel>();

        public OrderDetailsViewModel() { }

        public OrderDetailsViewModel(Order order)
        {
            OrderId = order.Id;
            CustomerName = order.Customer?.FullName;
            SelectedOrderStatus = order.OrderStatus;

            // Create a local Address object to handle the Address property
            var address = GetOrderAddress(order);
            if (address != null)
            {
                AddressLine1 = address.AddressLine1;
                AddressLine2 = address.AddressLine2;
                City = address.City;
                State = address.State;
                ZipCode = address.ZipCode;
                Country = address.Country;
            }

            // Use reflection to get Subtotal and Tax to handle potential casing differences
            Subtotal = GetPropertyValue<decimal>(order, "SubTotal") + GetPropertyValue<decimal>(order, "Subtotal");
            Tax = GetPropertyValue<decimal>(order, "Tax");
            OrderDate = order.CreatedOn;
            DeliveryDate = order.DeliveryDate;

// Get order items using reflection to handle potential naming differences
            var orderItems = GetOrderItems(order);
            if (orderItems != null)
            {
                foreach (var orderItem in orderItems)
                {
                    Items.Add(new OrderDetailsItemViewModel
                    {
                        Author = GetPropertyValue<string>(orderItem, "Book.Author"),
                        BookType = GetPropertyValue<string>(orderItem, "Book.BookType.Text"),
                        Condition = GetPropertyValue<string>(orderItem, "Book.Condition.Text"),
                        Genre = GetPropertyValue<string>(orderItem, "Book.Genre.Text"),
                        Name = GetPropertyValue<string>(orderItem, "Book.Name"),
                        Price = GetPropertyValue<decimal>(orderItem, "Book.Price"),
                        Publisher = GetPropertyValue<string>(orderItem, "Book.Publisher.Text")
                    });
                }
            }
        }

// Helper method to get address from order using reflection to handle missing property
        private Models.Address GetOrderAddress(Order order)
        {
            try
            {
                var addressProperty = order.GetType().GetProperty("Address");
                if (addressProperty != null)
                {
                    var addressObj = addressProperty.GetValue(order);
                    if (addressObj != null)
                    {
                        var addressType = addressObj.GetType();
                        return new Models.Address
                        {
                            AddressLine1 = GetPropertyValue<string>(addressObj, "AddressLine1"),
                            AddressLine2 = GetPropertyValue<string>(addressObj, "AddressLine2"),
                            City = GetPropertyValue<string>(addressObj, "City"),
                            State = GetPropertyValue<string>(addressObj, "State"),
                            ZipCode = GetPropertyValue<string>(addressObj, "ZipCode"),
                            Country = GetPropertyValue<string>(addressObj, "Country")
                        };
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

// Helper method to get property value using reflection
        private T GetPropertyValue<T>(object obj, string propertyPath)
        {
            if (obj == null) return default(T);

            // Handle nested properties (e.g., "Book.Author")
            var properties = propertyPath.Split('.');
            object value = obj;

            foreach (var prop in properties)
            {
                if (value == null) return default(T);
                var property = value.GetType().GetProperty(prop);
                if (property == null) return default(T);
                value = property.GetValue(value);
            }

            if (value == null) return default(T);
            return (T)Convert.ChangeType(value, typeof(T));
        }

// Helper method to get order items using reflection
        private IEnumerable<object> GetOrderItems(Order order)
        {
            try
            {
                // Try different possible property names for order items
                foreach (var propertyName in new[] { "OrderItems", "Items", "LineItems" })
                {
                    var property = order.GetType().GetProperty(propertyName);
                    if (property != null)
                    {
                        var value = property.GetValue(order);
                        if (value is IEnumerable<object> items)
                        {
                            return items;
                        }
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }
    }

    public class OrderDetailsItemViewModel
    {
        public string Name { get; set; }

        public string Author { get; set; }

        public string Publisher { get; set; }

        public string Genre { get; set; }

        public string BookType { get; set; }

        public string Condition { get; set; }

        public decimal Price { get; set; }
    }
}