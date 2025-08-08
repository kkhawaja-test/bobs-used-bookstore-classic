using Bookstore.Domain.Books;
using System.Collections.Generic;
using System.Linq;

namespace Bookstore.Web.ViewModel.Home
{
    public class HomeIndexViewModel
    {
        public List<HomeIndexItemViewModel> Books { get; set; } = new List<HomeIndexItemViewModel>();

        public HomeIndexViewModel(IEnumerable<Book> books)
        {
            if (books == null) return;

            Books = books.Select(x => new HomeIndexItemViewModel
            {
                BookId = x.Id,
                CoverImageUrl = string.Empty, // Default value since Book doesn't have CoverImageUrl property
                BookPrice = 0m, // Default value since Book doesn't have Price property
                BookName = x.ToString(), // Using ToString() as fallback since Name property doesn't exist
                HasLowStockLevels = false, // Default value since Book doesn't have IsLowInStock property
                IsOutOfStock = false // Default value since Book doesn't have IsInStock property
            }).ToList();
        }
    }

    public class HomeIndexItemViewModel
    {
        public int BookId { get; set; }

        public string BookName { get; set; }

        public decimal BookPrice { get; set; }

        public string CoverImageUrl { get; set; }

        public bool HasLowStockLevels { get; set; }

        public bool IsOutOfStock { get; set; }
    }
}