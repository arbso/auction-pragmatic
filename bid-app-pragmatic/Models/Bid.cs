using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace bid_app_pragmatic.Models
{
    public class Bid
    {
        [Key]
        public int BidId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime BidDate { get; set; } = DateTime.Now;

        [Required]
        public int AuctionId { get; set; }

        [Required]
        public int BidderId { get; set; }

        [ForeignKey("AuctionId")]
        public virtual Auction Auction { get; set; }

        [ForeignKey("BidderId")]
        public virtual User Bidder { get; set; }
    }
}
