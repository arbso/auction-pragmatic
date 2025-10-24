using bid_app_pragmatic.Models;
using bid_app_pragmatic.Repositories.Interfaces;
using bid_app_pragmatic.Services.Interfaces;
using bid_app_pragmatic.ViewModels.AuctionWebsite.ViewModels;
using bid_app_pragmatic.ViewModels;

namespace bid_app_pragmatic.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                if (await _userRepository.UsernameExistsAsync(model.Username))
                {
                    _logger.LogWarning($"Registration failed: Username '{model.Username}' already exists");
                    throw new InvalidOperationException("Username already exists");
                }

                if (await _userRepository.EmailExistsAsync(model.Email))
                {
                    _logger.LogWarning($"Registration failed: Email '{model.Email}' already exists");
                    throw new InvalidOperationException("Email already exists");
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PasswordHash = HashPassword(model.Password),
                    Wallet = 1000.00m,
                    CreatedAt = DateTime.Now
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();

                _logger.LogInformation($"User registered successfully: {user.Username}");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during registration for username: {model.Username}");
                throw;
            }
        }

        public async Task<User> LoginAsync(LoginViewModel model)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(model.Username);

                if (user == null)
                {
                    _logger.LogWarning($"Login failed: User '{model.Username}' not found");
                    return null;
                }

                if (!VerifyPassword(model.Password, user.PasswordHash))
                {
                    _logger.LogWarning($"Login failed: Invalid password for user '{model.Username}'");
                    return null;
                }

                _logger.LogInformation($"User logged in successfully: {user.Username}");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during login for username: {model.Username}");
                throw;
            }
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                return user != null && VerifyPassword(password, user.PasswordHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating user: {username}");
                throw;
            }
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
