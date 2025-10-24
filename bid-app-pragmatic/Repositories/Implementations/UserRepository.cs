using Microsoft.EntityFrameworkCore;
using bid_app_pragmatic.Data;
using bid_app_pragmatic.Models;
using bid_app_pragmatic.Repositories.Interfaces;

namespace bid_app_pragmatic.Repositories.Implementations
{
     public class UserRepository : IUserRepository
     {
            private readonly AuctionDbContext _context;
            private readonly ILogger<UserRepository> _logger;

            public UserRepository(AuctionDbContext context, ILogger<UserRepository> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<User> GetByIdAsync(int id)
            {
                try
                {
                    return await _context.Users.FindAsync(id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error getting user by id: {id}");
                    throw;
                }
            }

            public async Task<User> GetByUsernameAsync(string username)
            {
                try
                {
                    return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error getting user by username: {username}");
                    throw;
                }
            }

            public async Task<User> GetByEmailAsync(string email)
            {
                try
                {
                    return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error getting user by email: {email}");
                    throw;
                }
            }

            public async Task<IEnumerable<User>> GetAllAsync()
            {
                try
                {
                    return await _context.Users.ToListAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting all users");
                    throw;
                }
            }

            public async Task AddAsync(User user)
            {
                try
                {
                    await _context.Users.AddAsync(user);
                    _logger.LogInformation($"User added: {user.Username}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error adding user: {user.Username}");
                    throw;
                }
            }

            public async Task UpdateAsync(User user)
            {
                try
                {
                    _context.Users.Update(user);
                    _logger.LogInformation($"User updated: {user.Username}");
                    await Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating user: {user.Username}");
                    throw;
                }
            }

            public async Task<bool> UsernameExistsAsync(string username)
            {
                try
                {
                    return await _context.Users.AnyAsync(u => u.Username == username);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error checking username existence: {username}");
                    throw;
                }
            }

            public async Task<bool> EmailExistsAsync(string email)
            {
                try
                {
                    return await _context.Users.AnyAsync(u => u.Email == email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error checking email existence: {email}");
                    throw;
                }
            }

            public async Task SaveChangesAsync()
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving changes to database");
                    throw;
                }
            }
        }
}

