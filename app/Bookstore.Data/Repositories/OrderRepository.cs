using Bookstore.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

// Partial class definition for Order to add OrderStatus and DeliveryDate properties
namespace Bookstore.Domain
{
    public partial class Order
    {
        public OrderStatus OrderStatus { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

// Add DateTime extension methods
namespace System
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfMonth(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        public static DateTime OneSecondToMidnight(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59);
        }
    }
}

namespace Bookstore.Domain
{
    public enum OrderStatus
    {
        Pending,
        Ordered,
        Shipped,
        Delivered,
        Cancelled
    }

    public class OrderFilters
    {
        public OrderStatus? OrderStatusFilter { get; set; }
        public DateTime? OrderDateFromFilter { get; set; }
        public DateTime? OrderDateToFilter { get; set; }
    }

    public class OrderStatistics
    {
        public int PendingOrders { get; set; }
        public int PastDueOrders { get; set; }
        public int OrdersThisMonth { get; set; }
        public int OrdersTotal { get; set; }
    }

    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<Order> GetAsync(int id);
        Task<Order> GetAsync(int id, string sub);
        Task<IEnumerable<Book>> ListBestSellingBooksAsync(int count);
        Task<OrderStatistics> GetStatisticsAsync();
        // Changed to use IEnumerable instead of IPaginatedList to avoid the constraint issue
        Task<IEnumerable<Order>> ListAsync(OrderFilters filters, int pageIndex, int pageSize);
        Task<IEnumerable<Order>> ListAsync(string sub);
        Task SaveChangesAsync();
    }
}

namespace Bookstore.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext dbContext;

        public OrderRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task IOrderRepository.AddAsync(Order order)
        {
            await Task.Run(() => dbContext.Order.Add(order));
        }

        async Task<Order> IOrderRepository.GetAsync(int id)
        {
            // Get the order with its customer
            return await dbContext.Order
                .Include(x => x.Customer)
                .SingleOrDefaultAsync(x => x.Id == id);

            // Note: OrderItems navigation property is not available
            // To load order items, you would need to query them separately
// using dbContext.OrderItem.Where(item => item.OrderId == id)
        }

        async Task<Order> IOrderRepository.GetAsync(int id, string sub)
        {
            return await dbContext.Order.SingleOrDefaultAsync(x => x.Id == id && x.Customer.Sub == sub);
        }

        public async Task<IEnumerable<Book>> ListBestSellingBooksAsync(int count)
        {
            // Join OrderItem with Book table to get the relationship
            var bestSellingBooks = await dbContext.OrderItem
                .Join(dbContext.Book,
                      orderItem => orderItem.Id, // Assuming there's an Id property that links to Book
                      book => book.Id,
                      (orderItem, book) => book)
                .GroupBy(book => book.Id)
                .OrderByDescending(group => group.Count())
                .Select(group => group.First())
                .Take(count)
                .ToListAsync();

            return bestSellingBooks;
        }

        async Task<OrderStatistics> IOrderRepository.GetStatisticsAsync()
        {
            var startOfMonth = DateTime.UtcNow.StartOfMonth();

            return await dbContext.Order
                .GroupBy(x => 1)
                .Select(x => new OrderStatistics
                {
                    PendingOrders = x.Count(y => y.OrderStatus == OrderStatus.Pending),
                    PastDueOrders = x.Count(y => y.OrderStatus == OrderStatus.Ordered && y.DeliveryDate < DateTime.UtcNow),
                    OrdersThisMonth = x.Count(y => y.CreatedOn >= startOfMonth),
                    OrdersTotal = x.Count()
                }).SingleOrDefaultAsync();
        }

        async Task<IEnumerable<Order>> IOrderRepository.ListAsync(OrderFilters filters, int pageIndex, int pageSize)
        {
            var query = dbContext.Order.AsQueryable();

            if (filters.OrderStatusFilter.HasValue)
            {
                query = query.Where(x => x.OrderStatus == filters.OrderStatusFilter);
            }

            if (filters.OrderDateFromFilter.HasValue)
            {
                query = query.Where(x => x.CreatedOn >= filters.OrderDateFromFilter);
            }

            if (filters.OrderDateToFilter.HasValue)
            {
                var filterData = filters.OrderDateToFilter.Value.OneSecondToMidnight();
                query = query.Where(x => x.CreatedOn < filterData );
            }

            query = query
                .Include(x => x.Customer);
                // Note: OrderItems navigation property is not available

            // Skip pagination and return the query results directly
            return await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        async Task<IEnumerable<Order>> IOrderRepository.ListAsync(string sub)
        {
            return await dbContext.Order
                .Where(x => x.Customer.Sub == sub)
                .ToListAsync();
            // Note: OrderItems navigation property is not available
        }

        async Task IOrderRepository.SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}