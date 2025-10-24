namespace bid_app_pragmatic.ViewModels
{
    public class AuctionDetailsViewModel
    {
        public int AuctionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public DateTime EndDate { get; set; }
        public string CreatorUsername { get; set; }
        public bool IsEnded { get; set; }
        public List<BidViewModel> Bids { get; set; }
        public decimal UserWallet { get; set; }
        public int CurrentUserId { get; set; }
        public string WinnerUsername { get; set; }
    }

    public class BidViewModel
    {
        public string BidderUsername { get; set; }
        public decimal Amount { get; set; }
        public DateTime BidDate { get; set; }
    }
}
