using bid_app_pragmatic.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace bid_app_pragmatic.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuctionService _auctionService;
        private readonly IUserService _userService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IAuctionService auctionService,
            IUserService userService,
            ILogger<HomeController> logger)
        {
            _auctionService = auctionService;
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Loading home page - fetching auctions");

                await _auctionService.ProcessEndedAuctionsAsync();

                var userId = HttpContext.Session.GetInt32("UserId");
                _logger.LogInformation($"Current UserId from session: {userId}");

                if (userId.HasValue)
                {
                    var user = await _userService.GetUserByIdAsync(userId.Value);
                    if (user != null)
                    {
                        ViewBag.Username = user.Username;
                        ViewBag.Wallet = user.Wallet;
                        _logger.LogInformation($"User loaded: {user.Username}, Wallet: {user.Wallet}");
                    }
                }

                var auctions = await _auctionService.GetAllActiveAuctionsAsync();
                _logger.LogInformation($"Auctions fetched: {auctions?.Count() ?? 0}");

                if (auctions != null)
                {
                    foreach (var auction in auctions)
                    {
                        _logger.LogInformation($"Auction: {auction.Title}, EndDate: {auction.EndDate}, IsEnded: {auction.IsEnded}");
                    }
                }

                return View(auctions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");
                ViewBag.ErrorMessage = ex.Message;
                return View(new List<bid_app_pragmatic.Models.Auction>());
            }
        }
    }
}
