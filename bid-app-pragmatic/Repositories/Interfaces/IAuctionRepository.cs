using bid_app_pragmatic.Models;

namespace bid_app_pragmatic.Repositories.Interfaces
{
    public interface IAuctionRepository
    {
        Task<Auction> GetByIdAsync(int id);
        Task<Auction> GetByIdWithBidsAsync(int id);
        Task<IEnumerable<Auction>> GetAllActiveAsync();
        Task<IEnumerable<Auction>> GetByCreatorIdAsync(int creatorId);
        Task AddAsync(Auction auction);
        Task UpdateAsync(Auction auction);
        Task<IEnumerable<Auction>> GetEndedAuctionsAsync();
        Task SaveChangesAsync();
    }
}
