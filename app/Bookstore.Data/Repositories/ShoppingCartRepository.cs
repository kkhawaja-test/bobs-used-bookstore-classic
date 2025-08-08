using Bookstore.Domain;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq;

namespace Bookstore.Data.Repositories
{
    public interface IShoppingCartRepository
    {
        Task AddAsync(ShoppingCart shoppingCart);
        Task<ShoppingCart> GetAsync(string correlationId);
        Task SaveChangesAsync();
    }

    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly ApplicationDbContext dbContext;

        public ShoppingCartRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        async Task IShoppingCartRepository.AddAsync(ShoppingCart shoppingCart)
        {
            await Task.Run(() => dbContext.ShoppingCart.Add(shoppingCart));
        }

        async Task<ShoppingCart> IShoppingCartRepository.GetAsync(string correlationId)
        {
            return await dbContext.ShoppingCart
                .Include("ShoppingCartItems")
                .Include("ShoppingCartItems.Book")
                .SingleOrDefaultAsync(x => x.Id.ToString() == correlationId);
        }

        async Task IShoppingCartRepository.SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}