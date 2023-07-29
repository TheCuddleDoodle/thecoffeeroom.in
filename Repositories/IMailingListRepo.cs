using Coffeeroom.Models.Domain;

namespace Coffeeroom.Repositories
{
    public interface IMailingListRepo
    {
        Task<IEnumerable<Mail>> GetAllAsync();
        Task<Mail> GetAsync(int Id);
        Task<Mail> AddAsync(Mail mail);
        Task<Mail> UpdateAsync(Mail mail);
        Task<Mail> DeleteAsync(int Id);
    }
}
