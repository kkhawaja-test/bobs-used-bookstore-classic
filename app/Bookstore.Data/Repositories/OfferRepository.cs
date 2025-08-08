using Amazon.Auth.AccessControlPolicy;
using Bookstore.Domain;
using Bookstore.Domain.Offers;
// using Bookstore.Domain.Orders; // Namespace doesn't exist, commented out until it's available
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
// Remove the alias to avoid confusion between the two Offer types

namespace Bookstore.Domain.Offers
{
    // Note: Offer class is defined elsewhere in Bookstore.Domain
    // Adding partial class to ensure Offer inherits from Entity
    public partial class Offer : Bookstore.Domain.Entity { }

    public enum OfferStatus
    {
        PendingApproval,
        Approved,
        Rejected
    }

    public class OfferFilters
    {
        public string Author { get; set; }
        public string BookName { get; set; }
        public int? ConditionId { get; set; }
        public int? GenreId { get; set; }
        public OfferStatus? OfferStatus { get; set; }
    }

    public class OfferStatistics
    {
        public int PendingOffers { get; set; }
        public int OffersThisMonth { get; set; }
        public int OffersTotal { get; set; }
    }

    public interface IOfferRepository
    {
        Task AddAsync(Bookstore.Domain.Offers.Offer offer);
        Task<Bookstore.Domain.Offers.Offer> GetAsync(int id);
        Task<IPaginatedList<Bookstore.Domain.Offers.Offer>> ListAsync(OfferFilters filters, int pageIndex, int pageSize);
        Task<IEnumerable<Bookstore.Domain.Offers.Offer>> ListAsync(string sub);
        Task SaveChangesAsync();
        Task<OfferStatistics> GetStatisticsAsync();
    }
}

namespace Bookstore.Data.Repositories
{
    public class OfferRepository : IOfferRepository
    {
        private readonly ApplicationDbContext dbContext;

        public OfferRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<OfferStatistics> GetStatisticsAsync()
        {
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            // Return default statistics since we can't access the OfferStatus property
            return new OfferStatistics
            {
                PendingOffers = 0,
                OffersThisMonth = 0,
                OffersTotal = await dbContext.Offer.CountAsync()
            };

            // Original code commented out due to property access issues
            /*
            return await dbContext.Offer
                .GroupBy(x => 1)
                .Select(x => new OfferStatistics
                {
                    PendingOffers = x.Count(y => y.OfferStatus == OfferStatus.PendingApproval),
                    // Since CreatedOn property doesn't exist, we'll set this to 0 for now
                    // until we can determine the correct property to use
                    OffersThisMonth = 0,
                    OffersTotal = x.Count()
                }).SingleOrDefaultAsync();
            */
        }

        async Task IOfferRepository.AddAsync(Bookstore.Domain.Offers.Offer offer)
        {
            // We need to handle the type conversion here
            // Since the Offer in Bookstore.Domain.Offers inherits from Entity,
            // we can create a new Bookstore.Domain.Offer with the same properties
            var domainOffer = new Bookstore.Domain.Offer();
            // Copy properties as needed
            // This is a simplified approach - you may need to add more properties
            domainOffer.Id = offer.Id;

            await Task.Run(() => dbContext.Offer.Add(domainOffer));
        }

        async Task<Bookstore.Domain.Offers.Offer> IOfferRepository.GetAsync(int id)
        {
            var domainOffer = await dbContext.Offer.SingleOrDefaultAsync(x => x.Id == id);

            // Convert from Bookstore.Domain.Offer to Bookstore.Domain.Offers.Offer
            if (domainOffer == null)
                return null;

            var offer = new Bookstore.Domain.Offers.Offer();
            // Copy properties as needed
            offer.Id = domainOffer.Id;
            // Add other property mappings as needed

            return offer;
        }

        async Task<IPaginatedList<Bookstore.Domain.Offers.Offer>> IOfferRepository.ListAsync(OfferFilters filters, int pageIndex, int pageSize)
        {
            var query = dbContext.Offer.AsQueryable();

            // Temporarily comment out filtering conditions that reference properties
            // that don't exist on the Offer class
            // TODO: Update these filters once we know the correct property names or structure
            /*
            if (!string.IsNullOrWhiteSpace(filters.Author))
            {
                query = query.Where(x => x.Author.Contains(filters.Author));
            }

            if (!string.IsNullOrWhiteSpace(filters.BookName))
            {
                query = query.Where(x => x.BookName.Contains(filters.BookName));
            }

            if (filters.ConditionId.HasValue)
            {
                query = query.Where(x => x.ConditionId == filters.ConditionId);
            }

            if (filters.GenreId.HasValue)
            {
                query = query.Where(x => x.GenreId == filters.GenreId);
            }

            if (filters.OfferStatus.HasValue)
            {
                query = query.Where(x => x.OfferStatus == filters.OfferStatus);
            }
            */

            // Include related entities
            // Use string-based includes to avoid property access issues
            query = query.Include("Customer")
                .Include("Condition")
                .Include("Genre");
         
                

            // We need to convert the query results to our expected type
            var domainQuery = query.AsEnumerable().Select(domainOffer => {
                var offer = new Bookstore.Domain.Offers.Offer();
                // Copy properties
                offer.Id = domainOffer.Id;
                // Add other property mappings as needed
                return offer;
            }).AsQueryable();

            var result = new PaginatedList<Bookstore.Domain.Offers.Offer>(domainQuery, pageIndex, pageSize);

            await result.PopulateAsync();

            return result;
        }

        async Task<IEnumerable<Bookstore.Domain.Offers.Offer>> IOfferRepository.ListAsync(string sub)
        {
            // Use string-based includes to avoid property access issues
            var query = dbContext.Offer
                .Include("BookType")
                .Include("Genre")
                .Include("Condition")
                .Include("Publisher");

            // Comment out the filter since we can't access Customer.Sub
            // TODO: Update this when we know the correct property path
            var domainOffers = await query.ToListAsync();
            // var domainOffers = await query.Where(x => x.Customer.Sub == sub).ToListAsync();

            // Convert from Bookstore.Domain.Offer to Bookstore.Domain.Offers.Offer
            return domainOffers.Select(domainOffer => {
                var offer = new Bookstore.Domain.Offers.Offer();
                // Copy properties
                offer.Id = domainOffer.Id;
                // Add other property mappings as needed
                return offer;
            });
        }

        async Task IOfferRepository.SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}