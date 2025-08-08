using Bookstore.Domain;
// Commenting out missing namespace until proper reference is added
// using Bookstore.Domain.Offers;
using Bookstore.Domain.ReferenceData;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookstore.Web.Infrastructure.Paging
{
    public interface IPaginatedList<T> : IEnumerable<T>
    {
        int PageIndex { get; }
        int Count { get; }
        int TotalPages { get; }
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
        IEnumerable<int> GetPageList(int maxPages);
    }
}

namespace Bookstore.Domain
{
    public enum OfferStatus
    {
        Pending,
        Accepted,
        Rejected
    }

    public class Offer
    {
        public int Id { get; set; }
        public string BookName { get; set; }
        public string Author { get; set; }
        public ReferenceDataItem Genre { get; set; }
        public Customer Customer { get; set; }
        public OfferStatus OfferStatus { get; set; }
        public DateTime CreatedOn { get; set; }
        public decimal BookPrice { get; set; }
        public ReferenceDataItem Condition { get; set; }
    }

    public class Customer
    {
        public string FullName { get; set; }
    }
}



namespace Bookstore.Web.Areas.Admin.Models.Offers
{
using Bookstore.Web.Infrastructure.Paging;

    public class OfferFilters
    {
        public int? GenreId { get; set; }
        public int? ConditionId { get; set; }
        public string SearchTerm { get; set; }
        public OfferStatus? Status { get; set; }
    }

    public class OfferIndexViewModel : PaginatedViewModel
    {
        public OfferIndexViewModel(IPaginatedList<Offer> offers, IEnumerable<ReferenceDataItem> referenceData)
        {
            foreach (var offer in offers)
            {
                Items.Add(new OfferIndexItemViewModel
                {
                    OfferId = offer.Id,
                    BookName = offer.BookName,
                    Author = offer.Author,
                    Genre = offer.Genre.ToString(),
                    CustomerName = offer.Customer.FullName,
                    OfferStatus = (OfferStatus)(int)offer.OfferStatus,
                    OfferDate = offer.CreatedOn,
                    OfferPrice = offer.BookPrice,
                    Condition = offer.Condition.ToString()
                });
            }

            PageIndex = offers.PageIndex;
            PageSize = offers.Count;
            PageCount = offers.TotalPages;
            HasNextPage = offers.HasNextPage;
            HasPreviousPage = offers.HasPreviousPage;
            PaginationButtons = offers.GetPageList(5).ToList();

            // Since we can't access the properties directly, provide empty collections
            Genres = new List<SelectListItem>();
            BookConditions = new List<SelectListItem>();

            // If the ReferenceDataItem implementation is fixed later, uncomment and adjust this code:
            // Genres = referenceData.Where(x => /* filter condition */).Select(x => new SelectListItem { Value = x.ToString(), Text = x.ToString() });
            // BookConditions = referenceData.Where(x => /* filter condition */).Select(x => new SelectListItem { Value = x.ToString(), Text = x.ToString() });
        }

        public List<OfferIndexItemViewModel> Items { get; set; } = new List<OfferIndexItemViewModel>();

        public OfferFilters Filters { get; set; }

        public IEnumerable<SelectListItem> Genres { get; set; } = new List<SelectListItem>();

        public IEnumerable<SelectListItem> BookConditions { get; set; } = new List<SelectListItem>();
    }

    public class OfferIndexItemViewModel
    {
        public int OfferId { get; set; }

        public string BookName { get; set; }

        public string CustomerName { get; set; }

        public string Author { get; set; }

        public string Genre { get; set; }

        public OfferStatus OfferStatus { get; set; }

        public DateTime OfferDate { get; internal set; }

        public decimal OfferPrice { get; internal set; }

        public string Condition { get; internal set; }
    }
}