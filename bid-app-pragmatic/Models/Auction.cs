using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace bid_app_pragmatic.Models
{
    public class Auction
    {
        [Key]
        public int AuctionId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StartingPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? CurrentPrice { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsEnded { get; set; } = false;

        [Required]
        public int CreatorId { get; set; }

        public int? WinnerId { get; set; }

        [ForeignKey("CreatorId")]
        public virtual User Creator { get; set; }

        [ForeignKey("WinnerId")]
        public virtual User Winner { get; set; }

        public virtual ICollection<Bid> Bids { get; set; }
    }
}
