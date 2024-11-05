using EComWebAPI.Models;

namespace EComWebAPI.Repository.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category> UpdateAsync(Category entity);
    }
}
