using System.Data.Entity;
using System.Threading.Tasks;
using System.Linq;

namespace Bookstore.Domain.Customers
{
    public class Customer
    {
        public int Id { get; set; }
        public string Sub { get; set; }
    }

    public interface ICustomerRepository
    {
        Task AddAsync(Customer customer);
        Task<Customer> GetAsync(int id);
        Task<Customer> GetAsync(string sub);
        Task SaveChangesAsync();
    }
}

namespace Bookstore.Data.Repositories
{
    public class CustomerRepository : Bookstore.Domain.Customers.ICustomerRepository
    {
        private readonly ApplicationDbContext dbContext;

        public CustomerRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task Bookstore.Domain.Customers.ICustomerRepository.AddAsync(Bookstore.Domain.Customers.Customer customer)
        {
            // Create a new instance of Bookstore.Domain.Customer with the same properties
            var dbCustomer = new Bookstore.Domain.Customer
            {
                Id = customer.Id,
                Sub = customer.Sub
            };

            await Task.Run(() => dbContext.Customer.Add(dbCustomer));
        }

        async Task<Bookstore.Domain.Customers.Customer> Bookstore.Domain.Customers.ICustomerRepository.GetAsync(int id)
        {
            var dbCustomer = await dbContext.Customer.FindAsync(id);
            if (dbCustomer == null) return null;

            // Map from Bookstore.Domain.Customer to Bookstore.Domain.Customers.Customer
            return new Bookstore.Domain.Customers.Customer
            {
                Id = dbCustomer.Id,
                Sub = dbCustomer.Sub
            };
        }

        async Task<Bookstore.Domain.Customers.Customer> Bookstore.Domain.Customers.ICustomerRepository.GetAsync(string sub)
        {
            var dbCustomer = await dbContext.Customer.FirstOrDefaultAsync(x => x.Sub == sub);
            if (dbCustomer == null) return null;

            // Map from Bookstore.Domain.Customer to Bookstore.Domain.Customers.Customer
            return new Bookstore.Domain.Customers.Customer
            {
                Id = dbCustomer.Id,
                Sub = dbCustomer.Sub
            };
        }

        async Task Bookstore.Domain.Customers.ICustomerRepository.SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}