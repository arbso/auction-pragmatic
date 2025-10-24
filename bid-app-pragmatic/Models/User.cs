using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace bid_app_pragmatic.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 4)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Wallet { get; set; } = 1000.00m;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Auction> AuctionsCreated { get; set; }
        public virtual ICollection<Bid> Bids { get; set; }
    }
}
