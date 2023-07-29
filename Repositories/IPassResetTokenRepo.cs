using Coffeeroom.Models.Domain;

namespace Coffeeroom.Repositories
{
    public interface IPassResetTokenRepo
    {
        Task<PasswordToken> AddToken(PasswordToken passwordToken);
        Task ValidateToken(PasswordToken passwordToken);
        
    }
}
