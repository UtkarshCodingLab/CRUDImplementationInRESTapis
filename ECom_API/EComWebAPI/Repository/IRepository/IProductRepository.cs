using EComWebAPI.Models;

namespace EComWebAPI.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<Product> UpdateAsync(Product entity);
    }
}
