using Coffeeroom.Models.Domain;

namespace Coffeeroom.Repositories
{
    public interface IUserProfileRepo
    {
        Task<IEnumerable<UserProfile>> GetAllAsync();
        Task<UserProfile> GetAsync(int Id);
        Task<UserProfile> AddAsync(UserProfile userProfile);
        Task<UserProfile> UpdateAsync(UserProfile userProfile);
        Task<UserProfile> DeleteAsync(int Id);
    }
}
