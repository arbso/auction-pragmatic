using bid_app_pragmatic.Models;

namespace bid_app_pragmatic.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<decimal> GetUserWalletAsync(int userId);
        Task UpdateUserWalletAsync(int userId, decimal amount);
    }
}
