using bid_app_pragmatic.Data;
using bid_app_pragmatic.Models;
using bid_app_pragmatic.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bid_app_pragmatic.Repositories.Implementations
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly AuctionDbContext _context;
        private readonly ILogger<AuctionRepository> _logger;

        public AuctionRepository(AuctionDbContext context, ILogger<AuctionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Auction> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Auctions
                    .Include(a => a.Creator)
                    .Include(a => a.Winner)
                    .FirstOrDefaultAsync(a => a.AuctionId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting auction by id: {id}");
                throw;
            }
        }

        public async Task<Auction> GetByIdWithBidsAsync(int id)
        {
            try
            {
                return await _context.Auctions
                    .Include(a => a.Creator)
                    .Include(a => a.Winner)
                    .Include(a => a.Bids)
                        .ThenInclude(b => b.Bidder)
                    .FirstOrDefaultAsync(a => a.AuctionId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting auction with bids by id: {id}");
                throw;
            }
        }

        public async Task<IEnumerable<Auction>> GetAllActiveAsync()
        {
            try
            {
                return await _context.Auctions
                    .Include(a => a.Creator)
                    .Where(a => !a.IsEnded && a.EndDate > DateTime.Now)
                    .OrderBy(a => a.EndDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all active auctions");
                throw;
            }
        }

        public async Task<IEnumerable<Auction>> GetByCreatorIdAsync(int creatorId)
        {
            try
            {
                return await _context.Auctions
                    .Where(a => a.CreatorId == creatorId)
                    .OrderByDescending(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting auctions by creator id: {creatorId}");
                throw;
            }
        }

        public async Task AddAsync(Auction auction)
        {
            try
            {
                await _context.Auctions.AddAsync(auction);
                _logger.LogInformation($"Auction added: {auction.Title}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding auction: {auction.Title}");
                throw;
            }
        }

        public async Task UpdateAsync(Auction auction)
        {
            try
            {
                _context.Auctions.Update(auction);
                _logger.LogInformation($"Auction updated: {auction.Title}");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating auction: {auction.Title}");
                throw;
            }
        }

        public async Task<IEnumerable<Auction>> GetEndedAuctionsAsync()
        {
            try
            {
                return await _context.Auctions
                    .Include(a => a.Bids)
                        .ThenInclude(b => b.Bidder)
                    .Include(a => a.Creator)
                    .Where(a => !a.IsEnded && a.EndDate <= DateTime.Now)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ended auctions");
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
