using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;
using System.Dynamic;

namespace Bookstore.Web.Models.Search
{
    public class SearchDetailsViewModel
    {
        public int BookId { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        [Display(Name = "Title")]
        [DefaultValue("Title")]
        public string BookName { get; set; }

        [DefaultValue("Publisher not found")] public string PublisherName { get; set; }

        [DefaultValue("No Author")] public string Author { get; set; }

        public string ISBN { get; set; }

        [Display(Name = "Genre")] public string GenreName { get; set; }

        [Display(Name = "Type")] public string TypeName { get; set; }

        [Display(Name = "Condition")] public string ConditionName { get; set; }

        public string Url { get; set; }

        [Display(Name = "$$")] public decimal MinPrice { get; set; }

        public int Quantity { get; set; }

        public string Summary { get; set; }

        // Using dynamic object instead of Book to avoid the dependency
        public SearchDetailsViewModel(dynamic book)
        {
            try
            {
                BookName = book.Name;
                Author = book.Author;
                PublisherName = book.Publisher?.Text;
                ISBN = book.ISBN;
                GenreName = book.Genre?.Text;
                TypeName = book.BookType?.Text;
                ConditionName = book.Condition?.Text;
                Url = book.CoverImageUrl;
                MinPrice = book.Price;
                Quantity = book.Quantity;
                BookId = book.Id;
                Summary = book.Summary;
            }
            catch (Exception)
            {
                // Provide default values if properties can't be accessed
                BookName = "Unknown";
                Author = "Unknown";
                PublisherName = "Unknown";
                ISBN = "Unknown";
                GenreName = "Unknown";
                TypeName = "Unknown";
                ConditionName = "Unknown";
                Url = "";
                MinPrice = 0;
                Quantity = 0;
                BookId = 0;
                Summary = "";
            }
        }
    }
}