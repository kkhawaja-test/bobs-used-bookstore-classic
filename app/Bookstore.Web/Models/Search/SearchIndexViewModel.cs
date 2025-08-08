using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Bookstore.Domain.Books;
using Bookstore.Domain;
using System.Linq;
using System.Collections;

namespace Bookstore.Web.Models.Search
{
    // Local Book class to resolve the reference issue
    public class Book
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CoverImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public interface IPaginatedList<T> : IEnumerable<T>
    {
        int PageIndex { get; }
        int Count { get; }
        int TotalPages { get; }
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
        IEnumerable<int> GetPageList(int count);
    }

    public class PaginatedViewModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public List<int> PaginationButtons { get; set; }
    }

    public class SearchIndexViewModel : PaginatedViewModel
    {
        public string SearchString { get; set; }

        public string SortBy { get; set; }

        public List<SearchIndexItemViewModel> Books { get; set; } = new List<SearchIndexItemViewModel>();

        public SearchIndexViewModel(IPaginatedList<Book> books)
        {
            foreach (var book in books)
            {
                Books.Add(new SearchIndexItemViewModel
                {
                    BookId = book.Id,
                    BookName = book.Name,
                    ImageUrl = book.CoverImageUrl,
                    Price = book.Price,
                    Quantity = book.Quantity
                });
            }

            PageIndex = books.PageIndex;
            PageSize = books.Count;
            PageCount = books.TotalPages;
            HasNextPage = books.HasNextPage;
            HasPreviousPage = books.HasPreviousPage;
            PaginationButtons = books.GetPageList(5).ToList();
        }
    }

    public class SearchIndexItemViewModel
    {
        public int BookId { get; set; }

        [Display(Name = "Title")]
        [DefaultValue("Title")]
        public string BookName { get; set; }

        [DefaultValue("Publisher not found")]
        public string PublisherName { get; set; }

        [DefaultValue("No Author")]
        public string Author { get; set; }

        [Display(Name = "Genre")]
        public string GenreName { get; set; }

        [Display(Name = "Type")]
        public string TypeName { get; set; }

        [Display(Name = "Condition")]
        public string ConditionName { get; set; }

        public string ImageUrl { get; set; }

        [Display(Name = "$$")]
        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}