using bid_app_pragmatic.Models;
using bid_app_pragmatic.ViewModels;

namespace bid_app_pragmatic.Services.Interfaces
{
    public interface IAuctionService
    {
        Task<IEnumerable<Auction>> GetAllActiveAuctionsAsync();
        Task<Auction> GetAuctionByIdAsync(int id);
        Task<AuctionDetailsViewModel> GetAuctionDetailsAsync(int auctionId, int currentUserId);
        Task<Auction> CreateAuctionAsync(CreateAuctionViewModel model, int creatorId);
        Task ProcessEndedAuctionsAsync();
        Task<bool> PlaceBidAsync(int auctionId, int bidderId, decimal amount);
    }
}
