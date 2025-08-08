using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.Models.Checkout
{
    // Define the Order class and related classes needed for the view model
    public class Book
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string CoverImageUrl { get; set; }
    }

    public class OrderItem
    {
        public Book Book { get; set; }
        public int Quantity { get; set; }
    }

    public class Order
    {
        public IEnumerable<OrderItem> OrderItems { get; set; }
    }

    public class CheckoutFinishedViewModel
    {
        public IEnumerable<CheckoutFinishedItemViewModel> Items { get; set; } = new List<CheckoutFinishedItemViewModel>();

        public CheckoutFinishedViewModel(Order order)
        {
            Items = order.OrderItems.Select(x => new CheckoutFinishedItemViewModel
            {
                BookId = x.Book.Id,
                Bookname = x.Book.Name,
                Price = x.Book.Price,
                Quantity = x.Quantity,
                Url = x.Book.CoverImageUrl
            });
        }
    }

    public class CheckoutFinishedItemViewModel
    {
        public string Bookname { get; set; }

        public long BookId { get; set; }

        public int Quantity { get; set; }

        public string Url { get; set; }

        public decimal Price { get; set; }
    }
}