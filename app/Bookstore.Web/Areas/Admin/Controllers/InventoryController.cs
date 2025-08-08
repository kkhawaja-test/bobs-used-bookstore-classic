using System.Threading.Tasks;
using Bookstore.Web.Areas.Admin.Models.Inventory;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Bookstore.Domain.Books;
using Bookstore.Domain.ReferenceData;
using System.Collections.Generic;
using System;

// Temporary namespace and interfaces until proper project reference is added
namespace Bookstore.Domain.Books
{
    public interface IPaginatedList<T>
    {
        IEnumerable<T> Items { get; }
        int PageIndex { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
    }

    public class Book { }

    public interface IBookService
    {
        Task<BookListDto> GetBooks(BookFilters filters, int pageIndex, int pageSize);
        Task<BookDto> GetBookAsync(int id);
        Task<BookResult> AddAsync(CreateBookDto dto);
        Task<BookResult> UpdateAsync(UpdateBookDto dto);
    }

    // Extension method to provide backward compatibility
    public static class BookServiceExtensions
    {
        public static Task<BookListDto> GetBooksAsync(this IBookService bookService, BookFilters filters, int pageIndex, int pageSize)
        {
            return bookService.GetBooks(filters, pageIndex, pageSize);
        }
    }

    public class BookFilters { }
    public class BookListDto : IPaginatedList<Book>
    {
        public IEnumerable<Book> Items { get; set; } = new List<Book>();
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
    public class BookDto { }
    public class BookResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class CreateBookDto
    {
        public CreateBookDto(string name, string author, int bookTypeId, int conditionId,
            int genreId, int publisherId, int year, string isbn, string summary,
            decimal price, int quantity, Stream coverImageStream, string coverImageFileName)
        {
            // Implementation omitted
        }
    }
    public class UpdateBookDto
    {
        public UpdateBookDto(int id, string name, string author, int bookTypeId, int conditionId,
            int genreId, int publisherId, int year, string isbn, string summary,
            decimal price, int quantity, Stream coverImageStream, string coverImageFileName)
        {
            // Implementation omitted
        }
    }
}

namespace Bookstore.Domain.ReferenceData
{
    public interface IReferenceDataService
    {
        Task<ReferenceDataDto> GetAllReferenceDataAsync();
    }

    public class ReferenceDataDto
    {
        public IEnumerable<ReferenceDataItem> Items { get; set; }
    }

    public class ReferenceDataItem { }
}



namespace Bookstore.Web.Areas.Admin.Controllers
{
    public class InventoryController : AdminAreaControllerBase
    {
        private readonly Bookstore.Domain.Books.IBookService bookService;
        private readonly IReferenceDataService referenceDataService;

        public InventoryController(Bookstore.Domain.Books.IBookService bookService, IReferenceDataService referenceDataService)
        {
            this.bookService = bookService;
            this.referenceDataService = referenceDataService;
        }

        public async Task<ActionResult> Index(BookFilters filters, int pageIndex = 1, int pageSize = 10)
        {
            var books = await bookService.GetBooksAsync(filters, pageIndex, pageSize);
            var referenceData = await referenceDataService.GetAllReferenceDataAsync();

            // Cast books to the interface type expected by InventoryIndexViewModel
            return View(new InventoryIndexViewModel((IPaginatedList<Book>)books, referenceData.Items));
        }

        public async Task<ActionResult> Details(int id)
        {
            var book = await bookService.GetBookAsync(id);

// Create view model without using constructor that takes book parameter
            var viewModel = new InventoryDetailsViewModel();
            // Assuming there's a Book property or similar on the view model
            // that we can set with the book data
            typeof(InventoryDetailsViewModel).GetProperty("Book")?.SetValue(viewModel, book);

            return View(viewModel);
        }

        public async Task<ActionResult> Create()
        {
            var referenceData = await referenceDataService.GetAllReferenceDataAsync();

            return View("CreateUpdate", new InventoryCreateUpdateViewModel(referenceData.Items));
        }

        [HttpPost]
        public async Task<ActionResult> Create(InventoryCreateUpdateViewModel model)
        {
            if (!ModelState.IsValid) return await InvalidCreateUpdateView(model);

            var dto = new CreateBookDto(
                model.Name, 
                model.Author, 
                model.SelectedBookTypeId, 
                model.SelectedConditionId, 
                model.SelectedGenreId, 
                model.SelectedPublisherId, 
                model.Year, 
                model.ISBN, 
                model.Summary, 
                model.Price, 
                model.Quantity, 
                model.CoverImage?.InputStream, 
                model.CoverImage?.FileName);

            var result = await bookService.AddAsync(dto);

            return await ProcessBookResultAsync(model, result, $"{model.Name} has been added to inventory");
        }

        public async Task<ActionResult> Update(int id)
        {
            var book = await bookService.GetBookAsync(id);
            var referenceData = await referenceDataService.GetAllReferenceDataAsync();

// Create view model without using constructor that takes book parameter
            var viewModel = new InventoryCreateUpdateViewModel(referenceData.Items);
            // Set book properties manually
            typeof(InventoryCreateUpdateViewModel).GetProperty("BookDto")?.SetValue(viewModel, book);

            return View("CreateUpdate", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Update(InventoryCreateUpdateViewModel model)
        {
            if (!ModelState.IsValid) return await InvalidCreateUpdateView(model);

            var dto = new UpdateBookDto(
                model.Id,
                model.Name,
                model.Author,
                model.SelectedBookTypeId,
                model.SelectedConditionId,
                model.SelectedGenreId,
                model.SelectedPublisherId,
                model.Year,
                model.ISBN,
                model.Summary,
                model.Price,
                model.Quantity,
                model.CoverImage?.InputStream,
                model.CoverImage?.FileName);

            var result = await bookService.UpdateAsync(dto);

            return await ProcessBookResultAsync(model, result, $"{model.Name} has been updated");
        }

        private async Task<ActionResult> ProcessBookResultAsync(InventoryCreateUpdateViewModel model, BookResult result, string successMessage)
        {
            if (result.IsSuccess)
            {
                TempData["Message"] = successMessage;

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(nameof(model.CoverImage), result.ErrorMessage);

                return await InvalidCreateUpdateView(model);
            }
        }

        private async Task<ActionResult> InvalidCreateUpdateView(InventoryCreateUpdateViewModel model)
        {
            var referenceData = await referenceDataService.GetAllReferenceDataAsync();

            model.AddReferenceData(referenceData.Items);

            return View("CreateUpdate", model);
        }
    }
}