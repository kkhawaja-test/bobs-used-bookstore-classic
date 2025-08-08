using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Bookstore.Domain.Books;
using Bookstore.Domain.ReferenceData;
using Bookstore.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace Bookstore.Web.Areas.Admin.Models.Inventory
{
    public class InventoryCreateUpdateViewModel
    {
        public InventoryCreateUpdateViewModel() { }

        public InventoryCreateUpdateViewModel(IEnumerable<ReferenceDataItem> referenceDataItems)
        {
            AddReferenceData(referenceDataItems);
        }

        public InventoryCreateUpdateViewModel(IEnumerable<ReferenceDataItem> referenceDataItems, Book book) : this(referenceDataItems)
        {
            // TODO: Fix Author assignment - the Book class doesn't have an Author property
            // Author = book.Author;
            // CoverImageUrl property doesn't exist in Book class
            // TODO: Fix Id assignment - need to identify the correct property name in the Book class
            // Id = book.Id;
            // TODO: Fix ISBN assignment - the Book class doesn't have an ISBN property
            // ISBN = book.ISBN;
            // TODO: Fix Name assignment - the Book class doesn't have a Name property
            // Name = book.Name;
            // TODO: Fix Price assignment - the Book class doesn't have a Price property
            // Price = book.Price;
            // TODO: Fix Quantity assignment - the Book class doesn't have a Quantity property
            // Quantity = book.Quantity;
            // TODO: Fix BookTypeId assignment - the Book class doesn't have a BookTypeId property
            // SelectedBookTypeId = book.BookTypeId;
            // TODO: Fix ConditionId assignment - the Book class doesn't have a ConditionId property
            // SelectedConditionId = book.ConditionId;
            // TODO: Fix GenreId assignment - the Book class doesn't have a GenreId property
            // SelectedGenreId = book.GenreId;
            // TODO: Fix PublisherId assignment - the Book class doesn't have a PublisherId property
            // SelectedPublisherId = book.PublisherId;
            // TODO: Fix Summary assignment - the Book class doesn't have a Summary property
            // Summary = book.Summary;
            // TODO: Fix Year assignment - the Book class doesn't have a Year property
            // Year = book.Year.GetValueOrDefault();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Author { get; set; }

        public int Year { get; set; }

        [Required]
        public string ISBN { get; set; }

        public IEnumerable<SelectListItem> Publishers { get; set; } = new List<SelectListItem>();
        
        [Required]
        [DisplayName("Publisher")]
        public int SelectedPublisherId { get; set; }

        public IEnumerable<SelectListItem> BookTypes { get; set; } = new List<SelectListItem>();

        [Required]
        [DisplayName("Book Type")]
        public int SelectedBookTypeId { get; set; }

        public IEnumerable<SelectListItem> Genres { get; set; } = new List<SelectListItem>();
       
        [Required]
        [DisplayName("Genre")]
        public int SelectedGenreId { get; set; }

        public IEnumerable<SelectListItem> BookConditions { get; set; } = new List<SelectListItem>();
        
        [Required]
        [DisplayName("Condition")]
        public int SelectedConditionId { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;

        [MaxFileSize(2*1024*1024)]
        [ImageTypes(new string[] {".png", ".jpg", ".jpeg"})]
        [DisplayName("Cover image")]
        public HttpPostedFileBase CoverImage { get; set; }
        
        public string CoverImageUrl { get; set; }

        public string Summary { get; set; }

        public void AddReferenceData(IEnumerable<ReferenceDataItem> referenceDataItems)
        {
            BookConditions = referenceDataItems
                .Where(x => x.DataType == ReferenceDataType.Condition)
                .Select(x => new SelectListItem { Text = x.Text, Value = x.Id.ToString() });

            BookTypes = referenceDataItems
                .Where(x => x.DataType == ReferenceDataType.BookType)
                .Select(x => new SelectListItem{ Text = x.Text, Value = x.Id.ToString() });

            Genres = referenceDataItems
                .Where(x => x.DataType == ReferenceDataType.Genre)
                .Select(x => new SelectListItem{ Text = x.Text, Value = x.Id.ToString() });

            Publishers = referenceDataItems
                .Where(x => x.DataType == ReferenceDataType.Publisher)
                .Select(x => new SelectListItem{ Text = x.Text, Value = x.Id.ToString() });
        }
    }
}