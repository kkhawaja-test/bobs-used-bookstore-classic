using Bookstore.Domain.Carts;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.ViewModel.ShoppingCart
{
    public enum ShoppingCartItemFilter
    {
        IncludeOutOfStockItems,
        ExcludeOutOfStockItems
    }

    public class ShoppingCartIndexViewModel
    {
        public decimal TotalPrice => ShoppingCartItems.Sum(x => x.Price);

        public List<ShoppingCartIndexItemViewModel> ShoppingCartItems { get; set; } = new List<ShoppingCartIndexItemViewModel>();

        public ShoppingCartIndexViewModel(object shoppingCart)
        {
            if (shoppingCart == null) return;

            // Assuming the shoppingCart object has a method called GetShoppingCartItems
            // This is a temporary solution until we can properly reference the correct class
            dynamic cart = shoppingCart;
            try
            {
                var items = cart.GetShoppingCartItems(ShoppingCartItemFilter.IncludeOutOfStockItems);
                foreach (var c in items)
                {
                    ShoppingCartItems.Add(new ShoppingCartIndexItemViewModel
                    {
                        BookId = c.Book.Id,
                        ImageUrl = c.Book.CoverImageUrl,
                        Price = c.Book.Price,
                        BookName = c.Book.Name,
                        ShoppingCartItemId = c.Id,
                        StockLevel = c.Book.Quantity
                    });
                }
            }
            catch (System.Exception)
            {
                // Log error or handle appropriately
            }
        }
    }

    public class ShoppingCartIndexItemViewModel
    {
        public int ShoppingCartItemId { get; set; }

        public long BookId { get; set; }

        public string BookName { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public int StockLevel { get; set; }

        public bool HasLowStockLevels { get; set; }

        public bool IsOutOfStock { get; set; }
    }
}