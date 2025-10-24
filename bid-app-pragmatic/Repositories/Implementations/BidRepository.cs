using bid_app_pragmatic.Data;
using bid_app_pragmatic.Models;
using bid_app_pragmatic.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace bid_app_pragmatic.Repositories.Implementations
{
    public class BidRepository : IBidRepository
    {
        private readonly AuctionDbContext _context;
        private readonly ILogger<BidRepository> _logger;

        public BidRepository(AuctionDbContext context, ILogger<BidRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Bid> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Bids
                    .Include(b => b.Bidder)
                    .Include(b => b.Auction)
                    .FirstOrDefaultAsync(b => b.BidId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting bid by id: {id}");
                throw;
            }
        }

        public async Task<IEnumerable<Bid>> GetByAuctionIdAsync(int auctionId)
        {
            try
            {
                return await _context.Bids
                    .Include(b => b.Bidder)
                    .Where(b => b.AuctionId == auctionId)
                    .OrderByDescending(b => b.Amount)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting bids by auction id: {auctionId}");
                throw;
            }
        }

        public async Task<IEnumerable<Bid>> GetByBidderIdAsync(int bidderId)
        {
            try
            {
                return await _context.Bids
                    .Include(b => b.Auction)
                    .Where(b => b.BidderId == bidderId)
                    .OrderByDescending(b => b.BidDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting bids by bidder id: {bidderId}");
                throw;
            }
        }

        public async Task<Bid> GetHighestBidForAuctionAsync(int auctionId)
        {
            try
            {
                return await _context.Bids
                    .Include(b => b.Bidder)
                    .Where(b => b.AuctionId == auctionId)
                    .OrderByDescending(b => b.Amount)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting highest bid for auction id: {auctionId}");
                throw;
            }
        }

        public async Task AddAsync(Bid bid)
        {
            try
            {
                await _context.Bids.AddAsync(bid);
                _logger.LogInformation($"Bid added: Amount={bid.Amount}, AuctionId={bid.AuctionId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding bid");
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
