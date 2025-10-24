using bid_app_pragmatic.Models;
using bid_app_pragmatic.Repositories.Interfaces;
using bid_app_pragmatic.Services.Interfaces;

namespace bid_app_pragmatic.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                return await _userRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user by id: {id}");
                throw;
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _userRepository.GetByUsernameAsync(username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user by username: {username}");
                throw;
            }
        }

        public async Task<decimal> GetUserWalletAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                return user?.Wallet ?? 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting wallet for user: {userId}");
                throw;
            }
        }

        public async Task UpdateUserWalletAsync(int userId, decimal amount)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user != null)
                {
                    user.Wallet = amount;
                    await _userRepository.UpdateAsync(user);
                    await _userRepository.SaveChangesAsync();
                    _logger.LogInformation($"Wallet updated for user {userId}: ${amount}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating wallet for user: {userId}");
                throw;
            }
        }
    }
}
