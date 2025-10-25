using bid_app_pragmatic.Models;
using bid_app_pragmatic.Services.Interfaces;
using bid_app_pragmatic.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace bid_app_pragmatic.Controllers
{
    public class AuctionController : Controller
    {
        private readonly IAuctionService _auctionService;
        private readonly IUserService _userService;
        private readonly ILogger<AuctionController> _logger;

        public AuctionController(
            IAuctionService auctionService,
            IUserService userService,
            ILogger<AuctionController> logger)
        {
            _auctionService = auctionService;
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAuctionViewModel model)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                if (model.EndDate <= DateTime.Now)
                {
                    ModelState.AddModelError("EndDate", "End date must be in the future");
                    return View(model);
                }

                await _auctionService.CreateAuctionAsync(model, userId.Value);
                TempData["SuccessMessage"] = "Auction created successfully!";

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating auction");
                ModelState.AddModelError("", "An error occurred while creating the auction");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                await _auctionService.ProcessEndedAuctionsAsync();

                var viewModel = await _auctionService.GetAuctionDetailsAsync(id, userId.Value);
                if (viewModel == null)
                {
                    return NotFound();
                }

                var user = await _userService.GetUserByIdAsync(userId.Value);
                if (user != null)
                {
                    ViewBag.Username = user.Username;
                    ViewBag.Wallet = user.Wallet;
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading auction details for id: {id}");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceBid(int auctionId, decimal bidAmount)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (!userId.HasValue)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (bidAmount <= 0)
                {
                    TempData["ErrorMessage"] = "Bid amount must be greater than zero.";
                    _logger.LogWarning($"Invalid bid amount: {bidAmount}");
                    return RedirectToAction("Details", new { id = auctionId });
                }

                var result = await _auctionService.PlaceBidAsync(auctionId, userId.Value, bidAmount);

                if (result)
                {
                    TempData["SuccessMessage"] = $"Bid of ${bidAmount:N2} placed successfully!";
                    _logger.LogInformation($"Bid placed successfully: User {userId}, Auction {auctionId}, Amount {bidAmount}");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to place bid. Please try again.";
                    _logger.LogWarning($"Bid placement failed: User {userId}, Auction {auctionId}");
                }

                return RedirectToAction("Details", new { id = auctionId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, $"Bid placement validation error for auction {auctionId}");
                return RedirectToAction("Details", new { id = auctionId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while placing your bid. Please try again.";
                _logger.LogError(ex, $"Error placing bid on auction {auctionId}");
                return RedirectToAction("Details", new { id = auctionId });
            }
        }

    }
}
