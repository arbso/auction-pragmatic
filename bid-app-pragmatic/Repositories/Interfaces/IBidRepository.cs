using bid_app_pragmatic.Models;

namespace bid_app_pragmatic.Repositories.Interfaces
{
    public interface IBidRepository
    {
        Task<Bid> GetByIdAsync(int id);
        Task<IEnumerable<Bid>> GetByAuctionIdAsync(int auctionId);
        Task<IEnumerable<Bid>> GetByBidderIdAsync(int bidderId);
        Task<Bid> GetHighestBidForAuctionAsync(int auctionId);
        Task AddAsync(Bid bid);
        Task SaveChangesAsync();
    }
}
