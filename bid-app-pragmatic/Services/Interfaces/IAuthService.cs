using bid_app_pragmatic.Models;
using bid_app_pragmatic.ViewModels.AuctionWebsite.ViewModels;
using bid_app_pragmatic.ViewModels;

namespace bid_app_pragmatic.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterViewModel model);
        Task<User> LoginAsync(LoginViewModel model);
        Task<bool> ValidateUserAsync(string username, string password);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
