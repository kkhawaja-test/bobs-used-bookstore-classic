using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Bookstore.Web.Helpers;
using Bookstore.Domain.Books;
using Bookstore.Domain.Carts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Bookstore.Web.ViewModel.Search;

namespace Bookstore.Web.Controllers
{
    public class AddToShoppingCartDto
    {
        public Guid ShoppingCartId { get; }
        public int BookId { get; }
        public int Quantity { get; }

        public AddToShoppingCartDto(Guid shoppingCartId, int bookId, int quantity)
        {
            ShoppingCartId = shoppingCartId;
            BookId = bookId;
            Quantity = quantity;
        }
    }

    public class AddToWishlistDto
    {
        public Guid WishlistId { get; }
        public int BookId { get; }

        public AddToWishlistDto(Guid wishlistId, int bookId)
        {
            WishlistId = wishlistId;
            BookId = bookId;
        }
    }
}

namespace Bookstore.Web.ViewModel.Search
{
    public class SearchIndexViewModel
    {
        public IEnumerable<Book> Books { get; }

        public SearchIndexViewModel(IEnumerable<Book> books)
        {
            Books = books;
        }
    }

    public class SearchDetailsViewModel
    {
        public Book Book { get; }

        public SearchDetailsViewModel(Book book)
        {
            Book = book;
        }
    }
}

namespace Bookstore.Web.Controllers
{
    [AllowAnonymous]
    public class SearchController : Controller
    {
        private readonly IBookService inventoryService;
        private readonly IShoppingCartService shoppingCartService;

        public SearchController(IBookService inventoryService, IShoppingCartService shoppingCartService)
        {
            this.inventoryService = inventoryService;
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<ActionResult> Index(string searchString, string sortBy = "Name", int pageIndex = 1, int pageSize = 10)
        {
            var books = await inventoryService.GetBooksAsync(searchString, sortBy, pageIndex, pageSize);

            return View(new SearchIndexViewModel(books));
        }

        public async Task<ActionResult> Details(int id)
        {
            var book = await inventoryService.GetBookAsync(id);

            return View(new SearchDetailsViewModel(book));
        }

        public async Task<ActionResult> AddItemToShoppingCart(int bookId)
        {
            string cartIdString = Bookstore.Web.Helpers.HttpContextExtensions.GetShoppingCartCorrelationId(HttpContext);
            var dto = new AddToShoppingCartDto(Guid.Parse(cartIdString), bookId, 1);

            await shoppingCartService.AddToShoppingCartAsync(dto);

            this.SetNotification("Item added to shopping cart");

            return RedirectToAction("Index", "Search");
        }

        public ActionResult AddItemToWishlist(int bookId)
        {
            // Since the wishlist functionality appears to be missing in the interface,
            // we'll just redirect with a notification for now
            this.SetNotification("Wishlist functionality is currently unavailable");

            return RedirectToAction("Index", "Search");
        }
    }
}