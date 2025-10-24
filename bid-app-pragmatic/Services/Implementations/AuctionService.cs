using bid_app_pragmatic.Models;
using bid_app_pragmatic.Repositories.Interfaces;
using bid_app_pragmatic.Services.Interfaces;
using bid_app_pragmatic.ViewModels;

namespace bid_app_pragmatic.Services.Implementations
{
    public class AuctionService : IAuctionService
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IBidRepository _bidRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuctionService> _logger;

        public AuctionService(
            IAuctionRepository auctionRepository,
            IBidRepository bidRepository,
            IUserRepository userRepository,
            ILogger<AuctionService> logger)
        {
            _auctionRepository = auctionRepository;
            _bidRepository = bidRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Auction>> GetAllActiveAuctionsAsync()
        {
            try
            {
                return await _auctionRepository.GetAllActiveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all active auctions");
                throw;
            }
        }

        public async Task<Auction> GetAuctionByIdAsync(int id)
        {
            try
            {
                return await _auctionRepository.GetByIdWithBidsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting auction by id: {id}");
                throw;
            }
        }

        public async Task<AuctionDetailsViewModel> GetAuctionDetailsAsync(int auctionId, int currentUserId)
        {
            try
            {
                var auction = await _auctionRepository.GetByIdWithBidsAsync(auctionId);
                if (auction == null)
                {
                    return null;
                }

                var user = await _userRepository.GetByIdAsync(currentUserId);

                var viewModel = new AuctionDetailsViewModel
                {
                    AuctionId = auction.AuctionId,
                    Title = auction.Title,
                    Description = auction.Description,
                    StartingPrice = auction.StartingPrice,
                    CurrentPrice = auction.CurrentPrice ?? auction.StartingPrice,
                    EndDate = auction.EndDate,
                    CreatorUsername = auction.Creator.Username,
                    IsEnded = auction.IsEnded || auction.EndDate <= DateTime.Now,
                    UserWallet = user.Wallet,
                    CurrentUserId = currentUserId,
                    WinnerUsername = auction.Winner?.Username,
                    Bids = auction.Bids
                        .OrderByDescending(b => b.Amount)
                        .Select(b => new BidViewModel
                        {
                            BidderUsername = b.Bidder.Username,
                            Amount = b.Amount,
                            BidDate = b.BidDate
                        }).ToList()
                };

                return viewModel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting auction details for id: {auctionId}");
                throw;
            }
        }

        public async Task<Auction> CreateAuctionAsync(CreateAuctionViewModel model, int creatorId)
        {
            try
            {
                if (model.EndDate <= DateTime.Now)
                {
                    throw new InvalidOperationException("End date must be in the future");
                }

                var auction = new Auction
                {
                    Title = model.Title,
                    Description = model.Description,
                    StartingPrice = model.StartingPrice,
                    CurrentPrice = model.StartingPrice,
                    EndDate = model.EndDate,
                    CreatorId = creatorId,
                    CreatedAt = DateTime.Now,
                    IsEnded = false
                };

                await _auctionRepository.AddAsync(auction);
                await _auctionRepository.SaveChangesAsync();

                _logger.LogInformation($"Auction created: {auction.Title} by user {creatorId}");
                return auction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating auction: {model.Title}");
                throw;
            }
        }

        public async Task<bool> PlaceBidAsync(int auctionId, int bidderId, decimal amount)
        {
            try
            {
                var auction = await _auctionRepository.GetByIdWithBidsAsync(auctionId);
                if (auction == null)
                {
                    _logger.LogWarning($"Bid failed: Auction {auctionId} not found");
                    return false;
                }

                if (auction.IsEnded || auction.EndDate <= DateTime.Now)
                {
                    _logger.LogWarning($"Bid failed: Auction {auctionId} has ended");
                    return false;
                }

                if (auction.CreatorId == bidderId)
                {
                    _logger.LogWarning($"Bid failed: User {bidderId} cannot bid on their own auction");
                    throw new InvalidOperationException("You cannot bid on your own auction");
                }

                var currentPrice = auction.CurrentPrice ?? auction.StartingPrice;
                if (amount <= currentPrice)
                {
                    _logger.LogWarning($"Bid failed: Bid amount {amount} is not higher than current price {currentPrice}");
                    throw new InvalidOperationException($"Bid must be higher than current price of ${currentPrice}");
                }

                var bidder = await _userRepository.GetByIdAsync(bidderId);
                if (bidder.Wallet < amount)
                {
                    _logger.LogWarning($"Bid failed: User {bidderId} has insufficient funds");
                    throw new InvalidOperationException("Insufficient funds");
                }

                var bid = new Bid
                {
                    AuctionId = auctionId,
                    BidderId = bidderId,
                    Amount = amount,
                    BidDate = DateTime.Now
                };

                await _bidRepository.AddAsync(bid);

                auction.CurrentPrice = amount;
                await _auctionRepository.UpdateAsync(auction);

                await _bidRepository.SaveChangesAsync();

                _logger.LogInformation($"Bid placed: User {bidderId} bid ${amount} on auction {auctionId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error placing bid on auction {auctionId}");
                throw;
            }
        }

        public async Task ProcessEndedAuctionsAsync()
        {
            try
            {
                var endedAuctions = await _auctionRepository.GetEndedAuctionsAsync();

                foreach (var auction in endedAuctions)
                {
                    var highestBid = auction.Bids
                        .OrderByDescending(b => b.Amount)
                        .FirstOrDefault();

                    if (highestBid != null)
                    {
                        var winner = await _userRepository.GetByIdAsync(highestBid.BidderId);
                        var creator = await _userRepository.GetByIdAsync(auction.CreatorId);

                        winner.Wallet -= highestBid.Amount;
                        creator.Wallet += highestBid.Amount;

                        await _userRepository.UpdateAsync(winner);
                        await _userRepository.UpdateAsync(creator);

                        auction.WinnerId = highestBid.BidderId;

                        _logger.LogInformation($"Auction {auction.AuctionId} ended. Winner: {winner.Username}, Amount: ${highestBid.Amount}");
                    }
                    else
                    {
                        _logger.LogInformation($"Auction {auction.AuctionId} ended with no bids");
                    }

                    auction.IsEnded = true;
                    await _auctionRepository.UpdateAsync(auction);
                }

                if (endedAuctions.Any())
                {
                    await _auctionRepository.SaveChangesAsync();
                    _logger.LogInformation($"Processed {endedAuctions.Count()} ended auctions");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ended auctions");
                throw;
            }
        }
    }
}
